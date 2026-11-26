using bateka.games.Engine.ConnectFour;

namespace bateka.games.Engine.Tests.ConnectFour;

public sealed class ConnectFourBoardTests
{
    [Fact]
    public void A_disc_falls_to_the_lowest_free_row()
    {
        var board = new ConnectFourBoard();

        Assert.Equal(5, board.Drop(3, Disc.Red));
        Assert.Equal(4, board.Drop(3, Disc.Yellow));
        Assert.Equal(Disc.Yellow, board[4, 3]);
    }

    [Fact]
    public void A_full_column_takes_no_more_discs()
    {
        var board = new ConnectFourBoard();
        for (var i = 0; i < ConnectFourBoard.Rows; i++) board.Drop(0, i % 2 == 0 ? Disc.Red : Disc.Yellow);

        Assert.False(board.CanDrop(0));
        Assert.Equal(-1, board.Drop(0, Disc.Red));
        Assert.DoesNotContain(0, board.AvailableColumns());
    }

    [Fact]
    public void Undo_takes_the_top_disc_out()
    {
        var board = new ConnectFourBoard();
        board.Drop(2, Disc.Red);
        board.Drop(2, Disc.Yellow);

        board.Undo(2);

        Assert.Equal(Disc.None, board[4, 2]);
        Assert.Equal(Disc.Red, board[5, 2]);
    }

    [Fact]
    public void Four_across_wins()
    {
        var board = new ConnectFourBoard();
        for (var c = 1; c <= 4; c++) board.Drop(c, Disc.Red);

        Assert.Equal(Disc.Red, board.Winner());
        Assert.Equal(
            new HashSet<BoardCell> { new(5, 1), new(5, 2), new(5, 3), new(5, 4) },
            board.WinningCells().ToHashSet());
    }

    [Fact]
    public void Four_down_wins()
    {
        var board = new ConnectFourBoard();
        for (var i = 0; i < 4; i++) board.Drop(6, Disc.Yellow);

        Assert.Equal(Disc.Yellow, board.Winner());
    }

    [Fact]
    public void Four_on_a_diagonal_wins()
    {
        var board = new ConnectFourBoard();
        // a rising diagonal for Red from (5,0) to (2,3), Yellow underneath as filler
        board.Drop(0, Disc.Red);
        board.Drop(1, Disc.Yellow); board.Drop(1, Disc.Red);
        board.Drop(2, Disc.Yellow); board.Drop(2, Disc.Yellow); board.Drop(2, Disc.Red);
        board.Drop(3, Disc.Yellow); board.Drop(3, Disc.Yellow); board.Drop(3, Disc.Yellow);

        Assert.Equal(Disc.None, board.Winner());

        board.Drop(3, Disc.Red);

        Assert.Equal(Disc.Red, board.Winner());
    }

    [Fact]
    public void Three_in_a_row_is_not_a_win()
    {
        var board = new ConnectFourBoard();
        for (var c = 0; c < 3; c++) board.Drop(c, Disc.Red);

        Assert.Equal(Disc.None, board.Winner());
        Assert.Empty(board.WinningCells());
    }
}
