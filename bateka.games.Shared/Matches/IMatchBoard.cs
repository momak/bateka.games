namespace bateka.games.Shared.Matches;

/// <summary>One game's board for two people, built on the Engine's own board rules.</summary>
internal interface IMatchBoard
{
    MatchMoveCheck Check(int move);

    /// <summary>Plays a move that <see cref="Check"/> allowed.</summary>
    void Play(int move, MatchSide side);

    MatchSide? Winner();

    bool IsFull { get; }

    IReadOnlyList<MatchSide?> Cells();

    IReadOnlyList<int> WinningCells();
}
