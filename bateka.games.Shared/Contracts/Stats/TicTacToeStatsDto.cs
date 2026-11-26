using System.Text.Json.Serialization;

namespace bateka.games.Shared.Contracts.Stats;

/// <param name="TotalMoves">Moves by both sides, over all games.</param>
public sealed record TicTacToeStatsDto(int Wins, int Losses, int Draws, int TotalMoves)
{
    public static readonly TicTacToeStatsDto Empty = new(0, 0, 0, 0);

    [JsonIgnore] public int Total => Wins + Losses + Draws;

    [JsonIgnore] public int WinRate => Total == 0 ? 0 : (int)Math.Round((double)Wins / Total * 100);

    [JsonIgnore] public double AvgMoves => Total == 0 ? 0 : Math.Round((double)TotalMoves / Total, 1);
}
