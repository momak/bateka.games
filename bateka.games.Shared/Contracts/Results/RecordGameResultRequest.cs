using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Contracts.Results;

/// <summary>
/// One finished game. Which fields are needed depends on <see cref="Game"/> — the validator checks
/// that only the right ones are set:
/// Snake: Score, Length. Memory: Pairs, Moves, DurationMs. Tic Tac Toe and Connect Four (against
/// the computer only): Difficulty, Moves.
/// </summary>
public sealed class RecordGameResultRequest
{
    public GameKind Game { get; init; }
    public GameOutcome Outcome { get; init; }

    /// <summary>Tic Tac Toe and Connect Four.</summary>
    public Difficulty? Difficulty { get; init; }

    /// <summary>Memory: the board size.</summary>
    public int? Pairs { get; init; }

    /// <summary>Snake: points.</summary>
    public int? Score { get; init; }

    /// <summary>Snake: cells long at the end.</summary>
    public int? Length { get; init; }

    /// <summary>
    /// Memory: pairs turned over. Tic Tac Toe: moves by both sides. Connect Four: your own discs.
    /// </summary>
    public int? Moves { get; init; }

    /// <summary>Memory: how long it took.</summary>
    public int? DurationMs { get; init; }
}
