using bateka.games.Engine.Mastermind;
using bateka.games.Shared.Contracts.Results;

namespace bateka.games.Shared.Validation;

/// <summary>The engine's board, read back from a result request (validator and API both use it).</summary>
public static class ResultRequestBoards
{
    /// <summary>The Mastermind board the request describes, or null while a field is missing.</summary>
    public static MastermindSetup? MastermindBoard(RecordGameResultRequest request) =>
        request is { Columns: { } columns, Colours: { } colours, AllowRepeats: { } repeats, GuessLimit: { } limit }
            ? new MastermindSetup(columns, colours, repeats, limit)
            : null;
}
