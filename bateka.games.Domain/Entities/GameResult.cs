using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Domain.Entities;

/// <summary>
/// One finished game. Results are never updated: every stat, leaderboard and achievement is worked
/// out from these rows, so a new way of comparing players needs no data migration.
/// Only the fields of its own game are set (see RecordGameResultRequest).
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

    public DateTime PlayedAt { get; set; }
}
