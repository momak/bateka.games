using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Matches;

namespace bateka.games.Services.Matches;

public static class MatchMapper
{
    public static MatchDto ToDto(Match match) => new()
    {
        Id = match.Id,
        Game = match.Game,
        Status = match.Status,
        Creator = match.CreatorId,
        Opponent = match.OpponentId,
        Invited = match.InvitedId,
        FirstPlayer = match.FirstPlayerId,
        PlayerToMove = MatchPlayers.ToMove(match),
        Moves = [.. match.Moves],
        MoveSeconds = match.MoveSeconds,
        MoveDeadline = match.MoveDeadline,
        Winner = match.WinnerId,
        EndReason = match.EndReason,
        RematchId = match.RematchId,
        CreatedAt = match.CreatedAt,
        StartedAt = match.StartedAt,
        FinishedAt = match.FinishedAt
    };
}
