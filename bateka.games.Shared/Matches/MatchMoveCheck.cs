namespace bateka.games.Shared.Matches;

/// <summary>Whether a move can be played on the current position.</summary>
public enum MatchMoveCheck
{
    Ok,

    /// <summary>Not a square / column of this board.</summary>
    OffTheBoard,

    /// <summary>The square is taken, or the column is full.</summary>
    Taken,

    /// <summary>Someone already won, or the board is full.</summary>
    GameOver
}
