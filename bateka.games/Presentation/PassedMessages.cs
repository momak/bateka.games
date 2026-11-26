namespace bateka.games.Presentation;

/// <summary>The snackbar after a game that put you ahead of someone.</summary>
public static class PassedMessages
{
    private const int NamesShown = 2;

    /// <summary>"You passed ana!", "You passed ana and marko!", "You passed ana, marko and 2 more!" — or null.</summary>
    public static string? For(IReadOnlyList<string> passed) => passed.Count switch
    {
        0 => null,
        1 => $"You passed {passed[0]}!",
        2 => $"You passed {passed[0]} and {passed[1]}!",
        _ => $"You passed {string.Join(", ", passed.Take(NamesShown))} and {passed.Count - NamesShown} more!"
    };
}
