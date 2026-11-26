using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Stats;
using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Services.GameResults;

/// <summary>
/// The current user's finished games: records one (already validated by the Api's filter) and
/// answers with whether it's a personal best plus the updated stats.
/// </summary>
public sealed class GameResultService(IGameResultRepository results, ICurrentUser currentUser, TimeProvider clock)
{
    public async Task<RecordGameResultResponse> RecordAsync(RecordGameResultRequest request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var history = await results.ListForUserAsync(userId, ct);
        var before = GameStatsCalculator.For(history);

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

        results.Add(result);
        await results.SaveChangesAsync(ct);

        var isPersonalBest = PersonalBestRule.IsPersonalBest(result, before);
        var after = GameStatsCalculator.For([.. history, result]);
        return new RecordGameResultResponse(isPersonalBest, after);
    }

    public async Task<MyStatsDto> GetMineAsync(CancellationToken ct) =>
        GameStatsCalculator.For(await results.ListForUserAsync(currentUser.UserId, ct));
}
