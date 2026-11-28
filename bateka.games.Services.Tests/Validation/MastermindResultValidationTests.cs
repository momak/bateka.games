using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Validation;

namespace bateka.games.Services.Tests.Validation;

public sealed class MastermindResultValidationTests
{
    private readonly RecordGameResultRequestValidator _validator = new();

    private static RecordGameResultRequest Solve(int guesses = 4) => new()
    {
        Game = GameKind.Mastermind,
        Outcome = GameOutcome.Win,
        Moves = guesses,
        DurationMs = 60_000,
        Columns = 4,
        Colours = 6,
        AllowRepeats = true,
        GuessLimit = 10
    };

    [Fact]
    public void A_solve_with_its_board_is_valid()
    {
        Assert.True(_validator.Validate(Solve()).IsValid);
    }

    [Fact]
    public void A_loss_must_have_used_every_guess()
    {
        Assert.True(_validator.Validate(new RecordGameResultRequest
        {
            Game = GameKind.Mastermind, Outcome = GameOutcome.Loss, Moves = 10, DurationMs = 1000,
            Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10
        }).IsValid);

        Assert.False(_validator.Validate(new RecordGameResultRequest
        {
            Game = GameKind.Mastermind, Outcome = GameOutcome.Loss, Moves = 7, DurationMs = 1000,
            Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10
        }).IsValid);
    }

    public static TheoryData<RecordGameResultRequest> Invalid =>
    [
        // the browser may not send points
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Win, Moves = 4, DurationMs = 1000, Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10, Score = 9999 },
        // more guesses than allowed
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Win, Moves = 11, DurationMs = 1000, Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10 },
        // a board the game doesn't offer: 6 different colours out of 4
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Win, Moves = 4, DurationMs = 1000, Columns = 6, Colours = 4, AllowRepeats = false, GuessLimit = 10 },
        // no board at all
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Win, Moves = 4, DurationMs = 1000 },
        // Mastermind has no draw, and no difficulty
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Draw, Moves = 4, DurationMs = 1000, Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10 },
        new RecordGameResultRequest { Game = GameKind.Mastermind, Outcome = GameOutcome.Win, Moves = 4, DurationMs = 1000, Columns = 4, Colours = 6, AllowRepeats = true, GuessLimit = 10, Difficulty = Difficulty.Hard },
        // a Mastermind field on another game
        new RecordGameResultRequest { Game = GameKind.Snake, Outcome = GameOutcome.Finished, Score = 120, Length = 9, Columns = 4 }
    ];

    [Theory]
    [MemberData(nameof(Invalid))]
    public void Results_that_cant_come_from_the_game_are_refused(RecordGameResultRequest request)
    {
        Assert.False(_validator.Validate(request).IsValid);
    }
}
