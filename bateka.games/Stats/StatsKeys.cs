namespace bateka.games.Stats;

/// <summary>
/// localStorage keys for each game's stats on this device. Temporary: the backend increment moves
/// stats to the API (per Bateka user) and these go away.
/// </summary>
public static class StatsKeys
{
    public const string Snake = "snake_stats";
    public const string Memory = "memory_stats";
    public const string TicTacToe = "ttt_stats";
    public const string ConnectFour = "cf_stats";
}
