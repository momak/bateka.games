using System.Text.Json.Serialization;

namespace bateka.games.Shared.Contracts.Stats;

/// <param name="BestPoints">The best single solve (0 before the first).</param>
/// <param name="AverageGuesses">Over solved games only.</param>
public sealed record MastermindStatsDto(int GamesPlayed, int Solved, int BestPoints, double AverageGuesses)
{
    public static readonly MastermindStatsDto Empty = new(0, 0, 0, 0);

    [JsonIgnore] public int SolveRate => GamesPlayed == 0 ? 0 : (int)Math.Round(100.0 * Solved / GamesPlayed);
}
