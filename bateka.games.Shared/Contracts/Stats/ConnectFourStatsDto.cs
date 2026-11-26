namespace bateka.games.Shared.Contracts.Stats;

/// <summary>Games against the computer only.</summary>
public sealed record ConnectFourStatsDto(
    int GamesPlayed, int Wins, int Losses, int Draws, int CurrentStreak, int LongestWinStreak)
{
    public static readonly ConnectFourStatsDto Empty = new(0, 0, 0, 0, 0, 0);
}
