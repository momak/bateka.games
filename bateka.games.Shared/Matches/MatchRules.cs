using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Matches;

/// <summary>
/// The referee of online matches. The server replays the stored moves to check a new one, the
/// client replays them to draw the board — same code, so they can't disagree. Moves alternate:
/// even = <see cref="MatchSide.First"/>, odd = <see cref="MatchSide.Second"/>.
/// </summary>
public static class MatchRules
{
    public static bool Supports(GameKind game) => game is GameKind.TicTacToe or GameKind.ConnectFour;

    /// <summary>Whose turn it is after <paramref name="moveCount"/> moves.</summary>
    public static MatchSide SideToMove(int moveCount) => moveCount % 2 == 0 ? MatchSide.First : MatchSide.Second;

    /// <summary>The board after <paramref name="moves"/>. Throws if they aren't a legal game (stored moves always are).</summary>
    public static MatchPosition Replay(GameKind game, IReadOnlyList<int> moves)
    {
        var board = BoardFor(game);
        for (var i = 0; i < moves.Count; i++)
        {
            if (board.Check(moves[i]) != MatchMoveCheck.Ok)
            {
                throw new InvalidOperationException($"Move {i + 1} ({moves[i]}) isn't legal in this {game} match.");
            }

            board.Play(moves[i], SideToMove(i));
        }

        var winner = board.Winner();
        return new MatchPosition(
            board.Cells(),
            SideToMove(moves.Count),
            winner,
            IsDraw: winner is null && board.IsFull,
            board.WinningCells());
    }

    /// <summary>Whether <paramref name="move"/> can be played next.</summary>
    public static MatchMoveCheck Check(GameKind game, IReadOnlyList<int> moves, int move)
    {
        var board = BoardFor(game);
        for (var i = 0; i < moves.Count; i++)
        {
            board.Play(moves[i], SideToMove(i));
        }

        return board.Check(move);
    }

    private static IMatchBoard BoardFor(GameKind game) => game switch
    {
        GameKind.TicTacToe => new TicTacToeMatchBoard(),
        GameKind.ConnectFour => new ConnectFourMatchBoard(),
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Not playable online.")
    };
}
