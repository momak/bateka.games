namespace bateka.games.Services.Notes;

public static class NoteRules
{
    /// <summary>Someone who never marked notes as seen gets the last week's.</summary>
    public const int FirstVisitDays = 7;

    /// <summary>The newest this many — a long absence shouldn't fill the home page.</summary>
    public const int MaxNotes = 10;
}
