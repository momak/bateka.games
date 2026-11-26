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
        _ => $"{Wins(row.Value)} · {row.Tiebreak ?? 0:0}%"
    };

    /// <summary>"ana passed you on Snake — 430 pts vs your 412 pts".</summary>
    public static string Note(NoteDto note) =>
        $"{note.Player} passed you on {GameNames.Board(note.Game, note.Difficulty, note.Pairs)} — " +
        $"{Main(note.Game, note.TheirValue)} vs your {Main(note.Game, note.YourValue)}";

    private static string Main(GameKind game, int value) => game switch
    {
        GameKind.Snake => Points(value),
        GameKind.Memory => Moves(value),
        _ => Wins(value)
    };

    private static string Points(int value) => $"{value} pts";
    private static string Moves(int value) => value == 1 ? "1 move" : $"{value} moves";
    private static string Wins(int value) => value == 1 ? "1 win" : $"{value} wins";
}
