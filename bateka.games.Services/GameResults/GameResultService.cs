using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Boards;
using bateka.games.Services.Stats;
using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Services.GameResults;

/// <summary>
/// The current user's finished games: records one (already validated by the Api's filter) and
/// answers with whether it's a personal best, who it put you ahead of, and the updated stats.
/// </summary>
public sealed class GameResultService(IGameResultRepository results, ICurrentUser currentUser, TimeProvider clock)
{
    public async Task<RecordGameResultResponse> RecordAsync(RecordGameResultRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var result = new GameResult
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Game = request.Game,
            Outcome = request.Outcome,
            Difficulty = request.Difficulty,
            Pairs = request.Pairs,
            Score = request.Score,
            Length = request.Length,
            Moves = request.Moves,
            DurationMs = request.DurationMs,
            PlayedAt = clock.GetUtcNow().UtcDateTime
        };

        // before saving: "before" is everything already there
        var history = await results.ListForUserAsync(userId, ct);
        var boardBefore = await results.ListForBoardAsync(BoardKey.For(result), ct);
        var before = GameStatsCalculator.For(history);

        results.Add(result);
        await results.SaveChangesAsync(ct);

        return new RecordGameResultResponse(
            PersonalBestRule.IsPersonalBest(result, before),
            GameStatsCalculator.For([.. history, result]),
            PassCalculator.Passed(boardBefore, result));
    }

    public async Task<MyStatsDto> GetMineAsync(CancellationToken ct) =>
        GameStatsCalculator.For(await results.ListForUserAsync(currentUser.UserId, ct));
}
