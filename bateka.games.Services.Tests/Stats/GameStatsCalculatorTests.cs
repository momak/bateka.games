using bateka.games.Services.Stats;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Stats;
using R = bateka.games.Services.Tests.TestData.Results;

namespace bateka.games.Services.Tests.Stats;

public sealed class GameStatsCalculatorTests
{
    [Fact]
    public void No_results_means_empty_stats()
    {
        Assert.Equal(MyStatsDto.Empty, GameStatsCalculator.For([]));
    }

    [Fact]
    public void Snake_takes_the_best_score_the_longest_snake_and_the_average()
    {
        var stats = GameStatsCalculator.For([R.Snake(40, 6), R.Snake(120, 9), R.Snake(50, 12)]);

        Assert.Equal(new SnakeStatsDto(HighScore: 120, GamesPlayed: 3, LongestSnake: 12, AverageScore: 70), stats.Snake);
    }

    [Fact]
    public void Memory_keeps_the_fewest_moves_then_the_fastest_per_size()
    {
        var stats = GameStatsCalculator.For(
        [
            R.Memory(pairs: 6, moves: 14, seconds: 40),
            R.Memory(pairs: 6, moves: 10, seconds: 55),
            R.Memory(pairs: 6, moves: 10, seconds: 30),
            R.Memory(pairs: 3, moves: 4, seconds: 9)
        ]);

        Assert.Equal(4, stats.Memory.GamesPlayed);
        Assert.Equal([new MemoryBestDto(3, 4, 9), new MemoryBestDto(6, 10, 30)], stats.Memory.BestScores);
    }

    [Fact]
    public void Tic_tac_toe_counts_wins_losses_draws_and_moves()
    {
        var stats = GameStatsCalculator.For(
        [
            R.TicTacToe(GameOutcome.Win, 5),
            R.TicTacToe(GameOutcome.Win, 7),
            R.TicTacToe(GameOutcome.Loss, 6),
            R.TicTacToe(GameOutcome.Draw, 9)
        ]);

        Assert.Equal(new TicTacToeStatsDto(Wins: 2, Losses: 1, Draws: 1, TotalMoves: 27), stats.TicTacToe);
        Assert.Equal(50, stats.TicTacToe.WinRate);
        Assert.Equal(6.8, stats.TicTacToe.AvgMoves);
    }

    [Fact]
    public void Connect_four_streaks_follow_the_order_games_were_played()
    {
        var stats = GameStatsCalculator.For(
        [
            R.ConnectFour(GameOutcome.Win),
            R.ConnectFour(GameOutcome.Win),
            R.ConnectFour(GameOutcome.Win),
            R.ConnectFour(GameOutcome.Loss),
            R.ConnectFour(GameOutcome.Win),
            R.ConnectFour(GameOutcome.Draw),
            R.ConnectFour(GameOutcome.Win)
        ]);

        Assert.Equal(new ConnectFourStatsDto(GamesPlayed: 7, Wins: 5, Losses: 1, Draws: 1, CurrentStreak: 1, LongestWinStreak: 3),
            stats.ConnectFour);
    }

    [Fact]
    public void Each_game_only_counts_its_own_results()
    {
        var stats = GameStatsCalculator.For([R.Snake(30, 5), R.TicTacToe(GameOutcome.Win, 5)]);

        Assert.Equal(1, stats.Snake.GamesPlayed);
        Assert.Equal(1, stats.TicTacToe.Total);
        Assert.Equal(MemoryStatsDto.Empty, stats.Memory);
        Assert.Equal(ConnectFourStatsDto.Empty, stats.ConnectFour);
    }
}
