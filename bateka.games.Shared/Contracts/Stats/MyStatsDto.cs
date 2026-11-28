namespace bateka.games.Shared.Contracts.Stats;

/// <summary>Every game's stats for the current user.</summary>
public sealed record MyStatsDto(
    SnakeStatsDto Snake,
    MemoryStatsDto Memory,
    TicTacToeStatsDto TicTacToe,
    ConnectFourStatsDto ConnectFour,
    MastermindStatsDto Mastermind)
{
    public static readonly MyStatsDto Empty =
        new(SnakeStatsDto.Empty, MemoryStatsDto.Empty, TicTacToeStatsDto.Empty, ConnectFourStatsDto.Empty, MastermindStatsDto.Empty);
}
