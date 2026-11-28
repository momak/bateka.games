using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Domain.Entities;

/// <summary>
/// One finished game. Results are never updated: every stat, leaderboard and achievement is worked
/// out from these rows, so a new way of comparing players needs no data migration.
/// Only the fields of its own game are set (see RecordGameResultRequest). Mastermind: Score is the
/// solve's points (worked out by the API; null for a loss), Moves the guesses used.
/// </summary>
public class GameResult
{
    public Guid Id { get; set; }

    /// <summary>Username from Bateka.BFF (X-Bateka-User).</summary>
    public string UserId { get; set; } = string.Empty;

    public GameKind Game { get; set; }
    public GameOutcome Outcome { get; set; }

    public Difficulty? Difficulty { get; set; }
    public int? Pairs { get; set; }
    public int? Score { get; set; }
    public int? Length { get; set; }
    public int? Moves { get; set; }
    public int? DurationMs { get; set; }

    /// <summary>Mastermind's board, kept with the result so the points can be explained ("6×8, 4 guesses").</summary>
    public int? Columns { get; set; }
    public int? Colours { get; set; }
    public bool? AllowRepeats { get; set; }
    public int? GuessLimit { get; set; }

    public DateTime PlayedAt { get; set; }
}
