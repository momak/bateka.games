using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Validation;

namespace bateka.games.Services.Tests.Validation;

public sealed class RecordGameResultRequestValidatorTests
{
    private readonly RecordGameResultRequestValidator _validator = new();

    public static TheoryData<RecordGameResultRequest> Valid =>
    [
        new RecordGameResultRequest { Game = GameKind.Snake, Outcome = GameOutcome.Finished, Score = 120, Length = 9 },
        new RecordGameResultRequest { Game = GameKind.Memory, Outcome = GameOutcome.Win, Pairs = 6, Moves = 10, DurationMs = 30_000 },
        new RecordGameResultRequest { Game = GameKind.TicTacToe, Outcome = GameOutcome.Draw, Difficulty = Difficulty.Hard, Moves = 9 },
        new RecordGameResultRequest { Game = GameKind.ConnectFour, Outcome = GameOutcome.Loss, Difficulty = Difficulty.Medium, Moves = 3 }
    ];

    public static TheoryData<RecordGameResultRequest> Invalid =>
    [
        // Snake has no winner, and needs its length
        new RecordGameResultRequest { Game = GameKind.Snake, Outcome = GameOutcome.Win, Score = 120, Length = 9 },
        new RecordGameResultRequest { Game = GameKind.Snake, Outcome = GameOutcome.Finished, Score = 120 },
        // shorter than the snake starts
        new RecordGameResultRequest { Game = GameKind.Snake, Outcome = GameOutcome.Finished, Score = 0, Length = 2 },
        // a board size the game doesn't have, and fewer moves than pairs
        new RecordGameResultRequest { Game = GameKind.Memory, Outcome = GameOutcome.Win, Pairs = 9, Moves = 20, DurationMs = 1000 },
        new RecordGameResultRequest { Game = GameKind.Memory, Outcome = GameOutcome.Win, Pairs = 6, Moves = 5, DurationMs = 1000 },
        // a field of another game
        new RecordGameResultRequest { Game = GameKind.TicTacToe, Outcome = GameOutcome.Win, Difficulty = Difficulty.Easy, Moves = 5, Score = 10 },
        // no difficulty, and more moves than squares
        new RecordGameResultRequest { Game = GameKind.TicTacToe, Outcome = GameOutcome.Win, Moves = 5 },
        new RecordGameResultRequest { Game = GameKind.TicTacToe, Outcome = GameOutcome.Draw, Difficulty = Difficulty.Hard, Moves = 10 },
        // more of your discs than half the board
        new RecordGameResultRequest { Game = GameKind.ConnectFour, Outcome = GameOutcome.Win, Difficulty = Difficulty.Hard, Moves = 22 }
    ];

    [Theory]
    [MemberData(nameof(Valid))]
    public void A_result_the_game_can_produce_is_accepted(RecordGameResultRequest request)
    {
        Assert.True(_validator.Validate(request).IsValid);
    }

    [Theory]
    [MemberData(nameof(Invalid))]
    public void A_result_the_game_cannot_produce_is_refused(RecordGameResultRequest request)
    {
        Assert.False(_validator.Validate(request).IsValid);
    }
}
