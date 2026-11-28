using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Matches;

namespace bateka.games.Services.Tests.Matches;

public sealed class MatchRulesTests
{
    [Fact]
    public void Moves_alternate_starting_with_the_first_seat()
    {
        var position = MatchRules.Replay(GameKind.TicTacToe, [4, 0]);

        Assert.Equal(MatchSide.First, position.Cells[4]);
        Assert.Equal(MatchSide.Second, position.Cells[0]);
        Assert.Equal(MatchSide.First, position.ToMove);
        Assert.False(position.IsOver);
    }

    [Fact]
    public void Three_in_a_row_wins_tic_tac_toe()
    {
        // First: 0, 1, 2 (top row) — Second: 3, 4
        var position = MatchRules.Replay(GameKind.TicTacToe, [0, 3, 1, 4, 2]);

        Assert.Equal(MatchSide.First, position.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, position.WinningCells);
    }

    [Fact]
    public void A_full_board_without_a_line_is_a_draw()
    {
        // X O X / X O O / O X X
        var position = MatchRules.Replay(GameKind.TicTacToe, [0, 1, 2, 4, 3, 5, 7, 6, 8]);

        Assert.Null(position.Winner);
        Assert.True(position.IsDraw);
    }

    [Theory]
    [InlineData(4, MatchMoveCheck.Taken)]
    [InlineData(9, MatchMoveCheck.OffTheBoard)]
    [InlineData(8, MatchMoveCheck.Ok)]
    public void Tic_tac_toe_moves_are_checked(int move, MatchMoveCheck expected)
    {
        Assert.Equal(expected, MatchRules.Check(GameKind.TicTacToe, [4, 0], move));
    }

    [Fact]
    public void Nothing_can_be_played_after_a_win()
    {
        Assert.Equal(MatchMoveCheck.GameOver, MatchRules.Check(GameKind.TicTacToe, [0, 3, 1, 4, 2], 8));
    }

    [Fact]
    public void Four_on_top_of_each_other_wins_connect_four()
    {
        // First keeps dropping in column 0, Second in column 1
        var position = MatchRules.Replay(GameKind.ConnectFour, [0, 1, 0, 1, 0, 1, 0]);

        Assert.Equal(MatchSide.First, position.Winner);
        // bottom four slots of column 0: rows 2–5 of 7 columns
        Assert.Equal(new[] { 14, 21, 28, 35 }, position.WinningCells);
    }

    [Fact]
    public void Discs_fall_to_the_bottom()
    {
        var position = MatchRules.Replay(GameKind.ConnectFour, [3]);

        Assert.Equal(42, position.Cells.Count);
        Assert.Equal(MatchSide.First, position.Cells[5 * 7 + 3]);
        Assert.Null(position.Cells[3]);
    }

    [Fact]
    public void A_full_column_is_taken()
    {
        Assert.Equal(MatchMoveCheck.Taken, MatchRules.Check(GameKind.ConnectFour, [2, 2, 2, 2, 2, 2], 2));
        Assert.Equal(MatchMoveCheck.OffTheBoard, MatchRules.Check(GameKind.ConnectFour, [], 7));
    }

    [Fact]
    public void Stored_moves_that_break_the_rules_are_refused()
    {
        Assert.Throws<InvalidOperationException>(() => MatchRules.Replay(GameKind.TicTacToe, [4, 4]));
    }

    [Theory]
    [InlineData(GameKind.TicTacToe, true)]
    [InlineData(GameKind.ConnectFour, true)]
    [InlineData(GameKind.Snake, false)]
    [InlineData(GameKind.Memory, false)]
    public void Only_two_player_games_are_online(GameKind game, bool supported)
    {
        Assert.Equal(supported, MatchRules.Supports(game));
    }
}
