using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Leaderboards;
using bateka.games.Shared.Contracts.Notes;

namespace bateka.games.Presentation;

/// <summary>A board's numbers as text — what Value / Tiebreak mean differs per game.</summary>
public static class BoardFormat
{
    private static readonly string[] Medals = ["🥇", "🥈", "🥉"];

    public static string Rank(int rank) => rank <= Medals.Length ? Medals[rank - 1] : rank.ToString();

    public static string Value(GameKind game, LeaderboardRowDto row) => game switch
    {
        GameKind.Snake => Points(row.Value),
        GameKind.Memory => $"{Moves(row.Value)} · {TimeFormat.MinutesSeconds(row.Tiebreak ?? 0)}",
        GameKind.Mastermind => row.Solve is { } solve ? $"{Points(row.Value)} · {Solve(solve)}" : Points(row.Value),
        _ => $"{Wins(row.Value)} · {row.Tiebreak ?? 0:0}%"
    };

    /// <summary>"ana passed you on Snake — 430 pts vs your 412 pts".</summary>
    public static string Note(NoteDto note) =>
        $"{note.Player} passed you on {GameNames.Board(note.Game, note.Difficulty, note.Pairs)} — " +
        $"{MainValue(note.Game, note.TheirValue)} vs your {MainValue(note.Game, note.YourValue)}";

    private static string MainValue(GameKind game, int value) => game switch
    {
        GameKind.Snake or GameKind.Mastermind => Points(value),
        GameKind.Memory => Moves(value),
        _ => Wins(value)
    };

    private static string Points(int value) => $"{value} pts";

    /// <summary>"6×8 · 4 guesses", "4×6 no repeats · 1 guess".</summary>
    private static string Solve(MastermindSolveDto solve)
    {
        var repeats = solve.AllowRepeats ? string.Empty : " no repeats";
        var guesses = solve.Guesses == 1 ? "1 guess" : $"{solve.Guesses} guesses";
        return $"{solve.Columns}×{solve.Colours}{repeats} · {guesses}";
    }
    private static string Moves(int value) => value == 1 ? "1 move" : $"{value} moves";
    private static string Wins(int value) => value == 1 ? "1 win" : $"{value} wins";
}
