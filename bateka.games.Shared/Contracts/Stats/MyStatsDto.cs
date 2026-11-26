namespace bateka.games.Shared.Contracts.Stats;

/// <summary>Every game's stats for the current user.</summary>
public sealed record MyStatsDto(
    SnakeStatsDto Snake,
    MemoryStatsDto Memory,
    TicTacToeStatsDto TicTacToe,
    ConnectFourStatsDto ConnectFour)
{
    public static readonly MyStatsDto Empty =
        new(SnakeStatsDto.Empty, MemoryStatsDto.Empty, TicTacToeStatsDto.Empty, ConnectFourStatsDto.Empty);
}
