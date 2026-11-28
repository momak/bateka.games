using bateka.games.Engine.Mastermind;
using bateka.games.Engine.Memory;
using bateka.games.Engine.Snake;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Results;
using FluentValidation;

namespace bateka.games.Shared.Validation;

/// <summary>
/// A result must look like one the game can produce: the right outcome, only that game's fields,
/// and numbers inside the engine's limits. It can't prove the game was played honestly — results
/// come from the browser — but it keeps typos and broken clients off the leaderboards.
/// </summary>
public sealed class RecordGameResultRequestValidator : AbstractValidator<RecordGameResultRequest>
{
    private const string NotForThisGame = "Not used by this game.";
    private const string Required = "Required for this game.";

    public RecordGameResultRequestValidator()
    {
        RuleFor(x => x.Game).IsInEnum();
        RuleFor(x => x.Outcome).IsInEnum();

        // Mastermind's board fields mean nothing to the other games
        When(x => x.Game != GameKind.Mastermind, () =>
        {
            RuleFor(x => x.Columns).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Colours).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.AllowRepeats).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.GuessLimit).Null().WithMessage(NotForThisGame);
        });

        When(x => x.Game == GameKind.Mastermind, () =>
        {
            RuleFor(x => x.Outcome).Must(o => o is GameOutcome.Win or GameOutcome.Loss).WithMessage("Solved or not.");
            RuleFor(x => x.Columns).NotNull().WithMessage(Required);
            RuleFor(x => x.Colours).NotNull().WithMessage(Required);
            RuleFor(x => x.AllowRepeats).NotNull().WithMessage(Required);
            RuleFor(x => x.GuessLimit).NotNull().WithMessage(Required);
            RuleFor(x => x)
                .Must(x => ResultRequestBoards.MastermindBoard(x) is { } setup && MastermindRules.IsValidSetup(setup))
                .When(x => x.Columns is not null && x.Colours is not null && x.AllowRepeats is not null && x.GuessLimit is not null)
                .WithName(nameof(RecordGameResultRequest.Columns))
                .WithMessage("Not a board the game offers.");
            RuleFor(x => x.Moves).NotNull().WithMessage(Required)
                .Must((x, moves) => moves >= 1 && moves <= x.GuessLimit).WithMessage("Between one guess and the limit.");
            // a loss means every allowed guess was used
            RuleFor(x => x.Moves).Equal(x => x.GuessLimit).When(x => x.Outcome == GameOutcome.Loss)
                .WithMessage("A lost game used every guess.");
            RuleFor(x => x.DurationMs).NotNull().WithMessage(Required).InclusiveBetween(0, ResultLimits.MaxDurationMs);
            RuleFor(x => x.Difficulty).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Pairs).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Score).Null().WithMessage("Points are worked out by the server.");
            RuleFor(x => x.Length).Null().WithMessage(NotForThisGame);
        });

        When(x => x.Game == GameKind.Snake, () =>
        {
            RuleFor(x => x.Outcome).Equal(GameOutcome.Finished);
            RuleFor(x => x.Score).NotNull().WithMessage(Required).InclusiveBetween(0, ResultLimits.MaxSnakeScore);
            RuleFor(x => x.Length).NotNull().WithMessage(Required)
                .InclusiveBetween(SnakeRules.StartLength, ResultLimits.MaxSnakeLength);
            RuleFor(x => x.Difficulty).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Pairs).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Moves).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.DurationMs).Null().WithMessage(NotForThisGame);
        });

        When(x => x.Game == GameKind.Memory, () =>
        {
            // only won games are sent: an abandoned board has no result
            RuleFor(x => x.Outcome).Equal(GameOutcome.Win);
            RuleFor(x => x.Pairs).NotNull().WithMessage(Required)
                .InclusiveBetween(MemoryRules.MinPairs, MemoryRules.MaxPairs);
            RuleFor(x => x.Moves).NotNull().WithMessage(Required)
                .Must((request, moves) => moves >= request.Pairs && moves <= request.Pairs * ResultLimits.MaxMemoryMovesPerPair)
                .WithMessage("At least one move per pair.");
            RuleFor(x => x.DurationMs).NotNull().WithMessage(Required).InclusiveBetween(0, ResultLimits.MaxDurationMs);
            RuleFor(x => x.Difficulty).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Score).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Length).Null().WithMessage(NotForThisGame);
        });

        When(x => x.Game == GameKind.TicTacToe, () =>
        {
            RuleFor(x => x.Outcome).NotEqual(GameOutcome.Finished);
            RuleFor(x => x.Difficulty).NotNull().WithMessage(Required).IsInEnum();
            RuleFor(x => x.Moves).NotNull().WithMessage(Required)
                .InclusiveBetween(ResultLimits.MinTicTacToeMoves, ResultLimits.MaxTicTacToeMoves);
            RuleFor(x => x.Pairs).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Score).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Length).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.DurationMs).Null().WithMessage(NotForThisGame);
        });

        When(x => x.Game == GameKind.ConnectFour, () =>
        {
            RuleFor(x => x.Outcome).NotEqual(GameOutcome.Finished);
            RuleFor(x => x.Difficulty).NotNull().WithMessage(Required).IsInEnum();
            RuleFor(x => x.Moves).NotNull().WithMessage(Required)
                .InclusiveBetween(ResultLimits.MinConnectFourMoves, ResultLimits.MaxConnectFourMoves);
            RuleFor(x => x.Pairs).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Score).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.Length).Null().WithMessage(NotForThisGame);
            RuleFor(x => x.DurationMs).Null().WithMessage(NotForThisGame);
        });
    }
}
