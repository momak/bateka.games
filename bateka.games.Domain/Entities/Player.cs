namespace bateka.games.Domain.Entities;

/// <summary>What the app keeps about a player besides their results: when they last read their notes.</summary>
public class Player
{
    /// <summary>Username from Bateka.BFF (X-Bateka-User).</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>"Ana passed you" notes from before this are already read. Null = never.</summary>
    public DateTime? NotesSeenAt { get; set; }
}
