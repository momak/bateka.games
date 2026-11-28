namespace bateka.games.Services.Matches;

/// <summary>What the player is told when a match action is refused (shown as-is).</summary>
public static class MatchMessages
{
    public const string NotFound = "That match doesn't exist (or isn't yours).";
    public const string OwnInvite = "That's your own invite — send the link to someone else.";
    public const string AlreadyTaken = "Someone else already joined this match.";
    public const string NoLongerOpen = "This invite is no longer open.";
    public const string NotPlaying = "This match isn't being played.";
    public const string NotYourTurn = "It's not your turn.";
    public const string TimeUp = "Your time for this move ran out.";
    public const string OffTheBoard = "That's not on the board.";
    public const string Taken = "That spot is already taken.";
    public const string GameOver = "This game is already over.";
    public const string ChangedMeanwhile = "The match changed in the meantime — have another look.";
}
