using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Results;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;

namespace bateka.games.Services.Matches;

/// <summary>
/// Online matches for the current user. The server is the referee: every move is checked against
/// the stored moves with <see cref="MatchRules"/>, so a client can't play out of turn, on a taken
/// square or after the end. Every read first applies the clock (<see cref="MatchTimeouts"/>).
/// Each saved change is pushed to whoever watches the match (<see cref="IMatchNotifier"/>).
/// </summary>
public sealed class MatchService(
    IMatchRepository matches,
    IMatchNotifier notifier,
    ICurrentUser currentUser,
    TimeProvider clock)
{
    public async Task<MatchDto> CreateAsync(CreateMatchRequest request, CancellationToken ct)
    {
        var match = new Match
        {
            Id = Guid.NewGuid(),
            Game = request.Game,
            Status = MatchStatus.Waiting,
            CreatorId = currentUser.UserId,
            MoveSeconds = request.MoveSeconds,
            CreatedAt = Now
        };

        matches.Add(match);
        await matches.TrySaveChangesAsync(ct); // a new row can't conflict
        return MatchMapper.ToDto(match);
    }

    /// <summary>A waiting match is visible to anyone with the link (it's the invite); after that only to its players.</summary>
    public async Task<ServiceResult<MatchDto>> GetAsync(Guid id, CancellationToken ct)
    {
        var match = await LoadAsync(id, ct);
        if (match is null || !MatchPlayers.CanSee(match, currentUser.UserId)) return ServiceError.NotFound(MatchMessages.NotFound);

        return MatchMapper.ToDto(match);
    }

    public async Task<IReadOnlyList<MatchDto>> ListMineAsync(CancellationToken ct)
    {
        var mine = await matches.ListForUserAsync(currentUser.UserId, MatchLimits.RecentFinished, ct);

        var now = Now;
        var ended = mine.Where(match => MatchTimeouts.Apply(match, now)).ToList();

        // a conflict means the clock saved (and pushed) the same ending first
        if (ended.Count > 0 && await matches.TrySaveChangesAsync(ct))
        {
            foreach (var match in ended)
            {
                await NotifyAsync(match, ct);
            }
        }

        return mine.Select(MatchMapper.ToDto).ToList();
    }

    /// <summary>Takes the second seat; who moves first is decided now, at random.</summary>
    public async Task<ServiceResult<MatchDto>> JoinAsync(Guid id, CancellationToken ct)
    {
        var me = currentUser.UserId;
        var match = await LoadAsync(id, ct);
        if (match is null) return ServiceError.NotFound(MatchMessages.NotFound);

        if (match.CreatorId == me) return ServiceError.Invalid(MatchMessages.OwnInvite);
        if (match.Status == MatchStatus.Playing) return ServiceError.Conflict(MatchMessages.AlreadyTaken);
        if (match.Status != MatchStatus.Waiting) return ServiceError.Conflict(MatchMessages.NoLongerOpen);

        var now = Now;
        match.OpponentId = me;
        match.FirstPlayerId = Random.Shared.Next(2) == 0 ? match.CreatorId : me;
        match.Status = MatchStatus.Playing;
        match.StartedAt = now;
        match.MoveDeadline = now.AddSeconds(match.MoveSeconds);

        // two people opening the link at once: only one gets the seat
        if (!await matches.TrySaveChangesAsync(ct)) return ServiceError.Conflict(MatchMessages.AlreadyTaken);

        return await NotifyAsync(match, ct);
    }

    public async Task<ServiceResult<MatchDto>> MoveAsync(Guid id, MakeMoveRequest request, CancellationToken ct)
    {
        var me = currentUser.UserId;
        var match = await LoadAsync(id, ct);
        if (match is null || !MatchPlayers.IsPlayer(match, me)) return ServiceError.NotFound(MatchMessages.NotFound);

        if (match.Status != MatchStatus.Playing)
        {
            // LoadAsync just ended it on time if the deadline passed — tell the late player why
            return match.EndReason == MatchEndReason.TimedOut && match.WinnerId != me
                ? ServiceError.Conflict(MatchMessages.TimeUp)
                : ServiceError.Conflict(MatchMessages.NotPlaying);
        }

        if (MatchPlayers.ToMove(match) != me) return ServiceError.Invalid(MatchMessages.NotYourTurn);

        switch (MatchRules.Check(match.Game, match.Moves, request.Move))
        {
            case MatchMoveCheck.OffTheBoard: return ServiceError.Invalid(MatchMessages.OffTheBoard);
            case MatchMoveCheck.Taken: return ServiceError.Invalid(MatchMessages.Taken);
            case MatchMoveCheck.GameOver: return ServiceError.Conflict(MatchMessages.GameOver);
        }

        var now = Now;
        match.Moves = [.. match.Moves, request.Move];

        var position = MatchRules.Replay(match.Game, match.Moves);
        if (position.Winner is { } side)
        {
            MatchEnding.Finish(match, MatchPlayers.InSeat(match, side), MatchEndReason.Won, now);
        }
        else if (position.IsDraw)
        {
            MatchEnding.Finish(match, winnerId: null, MatchEndReason.Draw, now);
        }
        else
        {
            match.MoveDeadline = now.AddSeconds(match.MoveSeconds);
        }

        if (!await matches.TrySaveChangesAsync(ct)) return ServiceError.Conflict(MatchMessages.ChangedMeanwhile);

        return await NotifyAsync(match, ct);
    }

    /// <summary>While playing: you lose. While waiting (your own invite): the invite is withdrawn.</summary>
    public async Task<ServiceResult<MatchDto>> ResignAsync(Guid id, CancellationToken ct)
    {
        var me = currentUser.UserId;
        var match = await LoadAsync(id, ct);
        if (match is null || !MatchPlayers.IsPlayer(match, me)) return ServiceError.NotFound(MatchMessages.NotFound);

        switch (match.Status)
        {
            case MatchStatus.Playing:
                MatchEnding.Finish(match, MatchPlayers.Other(match, me), MatchEndReason.Resigned, Now);
                break;
            case MatchStatus.Waiting:
                MatchEnding.Cancel(match, MatchEndReason.Withdrawn, Now);
                break;
            default:
                return ServiceError.Conflict(MatchMessages.NotPlaying);
        }

        if (!await matches.TrySaveChangesAsync(ct)) return ServiceError.Conflict(MatchMessages.ChangedMeanwhile);

        return await NotifyAsync(match, ct);
    }

    private DateTime Now => clock.GetUtcNow().UtcDateTime;

    /// <summary>Pushes the saved match to its watchers and returns what was pushed.</summary>
    private async Task<MatchDto> NotifyAsync(Match match, CancellationToken ct)
    {
        var dto = MatchMapper.ToDto(match);
        await notifier.MatchChangedAsync(dto, ct);
        return dto;
    }

    /// <summary>The match with the clock applied (and saved, if that ended it).</summary>
    private async Task<Match?> LoadAsync(Guid id, CancellationToken ct)
    {
        var match = await matches.GetAsync(id, ct);
        if (match is null || !MatchTimeouts.Apply(match, Now)) return match;

        if (await matches.TrySaveChangesAsync(ct))
        {
            await NotifyAsync(match, ct);
            return match;
        }

        // the clock (or the other player) saved first: read what they saved
        return await matches.GetAsync(id, ct);
    }
}
