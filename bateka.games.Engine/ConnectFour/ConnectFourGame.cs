namespace bateka.games.Engine.ConnectFour;

/// <summary>
/// One game. Red (you) always starts. Against the computer, the page calls <see cref="Drop"/> for
/// your move, waits a moment, then <see cref="AiMove"/> while <see cref="IsAiTurn"/>.
/// </summary>
public sealed class ConnectFourGame(ConnectFourAi ai)
{
    public ConnectFourBoard Board { get; } = new();

    public ConnectFourMode Mode { get; private set; } = ConnectFourMode.VersusAi;

    public Difficulty Difficulty { get; private set; } = Difficulty.Hard;

    /// <summary>Whose turn it is — or, once the game is over, who made the last move.</summary>
    public Disc CurrentDisc { get; private set; } = Disc.Red;

    public ConnectFourResult Result { get; private set; }

    public bool IsOver => Result != ConnectFourResult.None;

    public IReadOnlySet<BoardCell> WinningCells { get; private set; } = new HashSet<BoardCell>();

    public bool IsAiTurn => Mode == ConnectFourMode.VersusAi && CurrentDisc == Disc.Yellow && !IsOver;

    /// <summary>Your own discs dropped in this game (Red).</summary>
    public int RedMoves { get; private set; }

    public void Start(ConnectFourMode mode, Difficulty difficulty)
    {
        Board.Clear();
        Mode = mode;
        Difficulty = difficulty;
        CurrentDisc = Disc.Red;
        Result = ConnectFourResult.None;
        WinningCells = new HashSet<BoardCell>();
        RedMoves = 0;
    }

    /// <summary>
    /// A person's move in <paramref name="column"/>. False when it's the computer's turn, the
    /// column is full, or the game is over.
    /// </summary>
    public bool Drop(int column)
    {
        if (IsOver || IsAiTurn) return false;

        return Play(column);
    }

    /// <summary>The computer's move. Returns the column it played, or -1 when it isn't its turn.</summary>
    public int AiMove()
    {
        if (!IsAiTurn) return -1;

        var column = ai.ChooseColumn(Board, Difficulty);
        return column >= 0 && Play(column) ? column : -1;
    }

    private bool Play(int column)
    {
        if (Board.Drop(column, CurrentDisc) < 0) return false;

        if (CurrentDisc == Disc.Red) RedMoves++;

        var winner = Board.Winner();
        if (winner != Disc.None)
        {
            WinningCells = Board.WinningCells();
            Result = winner == Disc.Red ? ConnectFourResult.RedWins : ConnectFourResult.YellowWins;
            return true; // CurrentDisc stays on the winner
        }

        if (Board.IsFull)
        {
            Result = ConnectFourResult.Draw;
            return true;
        }

        CurrentDisc = CurrentDisc == Disc.Red ? Disc.Yellow : Disc.Red;
        return true;
    }
}
