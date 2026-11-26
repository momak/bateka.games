using bateka.games.Engine.TicTacToe;
using bateka.games.Engine.Tests.TestDoubles;

namespace bateka.games.Engine.Tests.TicTacToe;

public sealed class TicTacToeGameTests
{
    private static TicTacToeGame NewGame(Random? random = null) =>
        new(new TicTacToeAi(random ?? new Random(3)));

    [Fact]
    public void The_player_cannot_play_a_taken_square()
    {
        var game = NewGame();

        Assert.True(game.PlayerMove(4));
        Assert.False(game.PlayerMove(4));
        Assert.Equal(1, game.Moves);
    }

    [Fact]
    public void Three_in_a_row_wins_and_marks_the_line()
    {
        var game = NewGame(new FixedRandom(0, 0, 0)); // Easy plays the first free square

        game.PlayerMove(0);
        game.AiMove(Difficulty.Easy);   // 1
        game.PlayerMove(3);
        game.AiMove(Difficulty.Easy);   // 2
        game.PlayerMove(6);

        Assert.Equal(TicTacToeResult.PlayerWin, game.Result);
        Assert.Equal([0, 3, 6], game.WinningLine);
        Assert.Equal(3, game.PlayerMoves);
        Assert.False(game.PlayerMove(8));
        Assert.Equal(-1, game.AiMove(Difficulty.Easy));
    }

    [Fact]
    public void Hard_takes_a_winning_square()
    {
        var ai = new TicTacToeAi(new Random(3));
        CellState[] board =
        [
            CellState.O, CellState.O, CellState.Empty,
            CellState.X, CellState.X, CellState.Empty,
            CellState.X, CellState.Empty, CellState.Empty
        ];

        Assert.Equal(2, ai.ChooseMove(board, Difficulty.Hard));
    }

    [Fact]
    public void Hard_blocks_the_player()
    {
        var ai = new TicTacToeAi(new Random(3));
        CellState[] board =
        [
            CellState.X, CellState.X, CellState.Empty,
            CellState.Empty, CellState.O, CellState.Empty,
            CellState.Empty, CellState.Empty, CellState.Empty
        ];

        Assert.Equal(2, ai.ChooseMove(board, Difficulty.Hard));
    }

    [Fact]
    public void Hard_never_loses_whatever_the_player_does()
    {
        var playerWins = CountPlayerWins(NewGame());

        Assert.Equal(0, playerWins);
    }

    [Fact]
    public void A_full_board_without_a_line_is_a_draw()
    {
        // ends as X O X / X O O / O X X. Easy picks "the n-th free square" from the fixed list.
        var game = NewGame(new FixedRandom(0, 1, 0, 0));

        game.PlayerMove(0); game.AiMove(Difficulty.Easy); // O on 1
        game.PlayerMove(2); game.AiMove(Difficulty.Easy); // O on 4
        game.PlayerMove(3); game.AiMove(Difficulty.Easy); // O on 5
        game.PlayerMove(7); game.AiMove(Difficulty.Easy); // O on 6
        game.PlayerMove(8);

        Assert.Equal(TicTacToeResult.Draw, game.Result);
        Assert.Null(game.WinningLine);
        Assert.Equal(9, game.Moves);
    }

    /// <summary>Plays every possible player line against Hard and counts the player's wins.</summary>
    private static int CountPlayerWins(TicTacToeGame start)
    {
        var wins = 0;
        Explore([]);
        return wins;

        void Explore(List<int> playerMoves)
        {
            var game = Replay(playerMoves);
            if (game is null) return;
            if (game.IsOver)
            {
                if (game.Result == TicTacToeResult.PlayerWin) wins++;
                return;
            }

            foreach (var square in TicTacToeBoard.EmptyCells(game.Board))
            {
                Explore([.. playerMoves, square]);
            }
        }

        TicTacToeGame? Replay(List<int> playerMoves)
        {
            var game = NewGame();
            foreach (var square in playerMoves)
            {
                if (!game.PlayerMove(square)) return null;
                if (!game.IsOver) game.AiMove(Difficulty.Hard);
            }

            return game;
        }
    }
}
