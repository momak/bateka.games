using bateka.games.Shared.Contracts.Stats;

namespace bateka.games.Shared.Contracts.Results;

/// <param name="IsPersonalBest">
/// A new best of your own: Snake's high score, or Memory's fewest moves / fastest time on that size.
/// </param>
/// <param name="Stats">Your stats including this game, so the page needn't ask again.</param>
/// <param name="Passed">Players this game put you ahead of on its board ("You passed Ana!").</param>
public sealed record RecordGameResultResponse(bool IsPersonalBest, MyStatsDto Stats, IReadOnlyList<string> Passed);
