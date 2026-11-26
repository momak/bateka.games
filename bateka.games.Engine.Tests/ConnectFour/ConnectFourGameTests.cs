using bateka.games.Engine.ConnectFour;
using bateka.games.Engine.Tests.TestDoubles;

namespace bateka.games.Engine.Tests.ConnectFour;

public sealed class ConnectFourGameTests
{
    private static ConnectFourGame Started(ConnectFourMode mode, Difficulty difficulty = Difficulty.Hard, Random? random = null)
    {
        var game = new ConnectFourGame(new ConnectFourAi(random ?? new Random(5)));
        game.Start(mode, difficulty);
        return game;
    }

    [Fact]
    public void Two_players_take_turns_starting_with_red()
    {
        var game = Started(ConnectFourMode.TwoPlayer);

        Assert.Equal(Disc.Red, game.CurrentDisc);
        game.Drop(0);
        Assert.Equal(Disc.Yellow, game.CurrentDisc);
        Assert.False(game.IsAiTurn);
        game.Drop(1);
        Assert.Equal(Disc.Red, game.CurrentDisc);
    }

    [Fact]
    public void Against_the_computer_you_cannot_move_on_its_turn()
    {
        var game = Started(ConnectFourMode.VersusAi);

        game.Drop(3);

        Assert.True(game.IsAiTurn);
        Assert.False(game.Drop(4));

        var column = game.AiMove();

        Assert.InRange(column, 0, ConnectFourBoard.Columns - 1);
        Assert.Equal(Disc.Red, game.CurrentDisc);
        Assert.Equal(-1, game.AiMove());
    }

    [Fact]
    public void Four_in_a_row_ends_the_game_with_the_winner_as_current_disc()
    {
        var game = Started(ConnectFourMode.TwoPlayer);

        foreach (var column in new[] { 0, 0, 1, 1, 2, 2, 3 })
        {
            game.Drop(column);
        }

        Assert.Equal(ConnectFourResult.RedWins, game.Result);
        Assert.Equal(Disc.Red, game.CurrentDisc);
        Assert.Equal(4, game.WinningCells.Count);
        Assert.Equal(4, game.RedMoves);
        Assert.False(game.Drop(4));
    }

    [Fact]
    public void Medium_blocks_three_in_a_row()
    {
        // Easy fallback would play column 0 (FixedRandom → first free), so a block is visible
        var game = Started(ConnectFourMode.VersusAi, Difficulty.Medium, new FixedRandom(0, 0, 0));

        game.Drop(1);
        game.AiMove();            // random → column 0
        game.Drop(2);
        game.AiMove();            // random → column 0
        game.Drop(3);             // red threatens 1-2-3-4

        var column = game.AiMove();

        Assert.Equal(4, column);
    }

    [Fact]
    public void Hard_takes_an_immediate_win()
    {
        var game = Started(ConnectFourMode.TwoPlayer);
        // yellow has three stacked in column 6; red elsewhere
        foreach (var column in new[] { 0, 6, 1, 6, 0, 6 })
        {
            game.Drop(column);
        }

        var ai = new ConnectFourAi(new Random(5));

        Assert.Equal(6, ai.ChooseColumn(game.Board, Difficulty.Hard));
    }

    [Fact]
    public void Hard_blocks_an_immediate_loss()
    {
        var game = Started(ConnectFourMode.TwoPlayer);
        // red has three across the bottom (1, 2, 3); yellow already closes the left end (0)
        foreach (var column in new[] { 1, 0, 2, 6, 3 })
        {
            game.Drop(column);
        }

        var ai = new ConnectFourAi(new Random(5));

        Assert.Equal(4, ai.ChooseColumn(game.Board, Difficulty.Hard));
    }
}
