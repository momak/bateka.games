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
