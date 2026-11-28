using bateka.games.Engine.Mastermind;
using bateka.games.Engine.Tests.TestDoubles;

namespace bateka.games.Engine.Tests.Mastermind;

public sealed class MastermindGameTests
{
    /// <summary>Medium (repeats allowed) takes one Next(6) per peg, so the code is exactly these.</summary>
    private static MastermindGame MediumWithCode(params int[] code)
    {
        var game = new MastermindGame(new FixedRandom(code));
        game.Start(Difficulty.Medium);
        return game;
    }

    [Fact]
    public void The_right_guess_solves_it()
    {
        var game = MediumWithCode(2, 0, 5, 1);

        Assert.Equal(MastermindGuessOutcome.Scored, game.Guess([0, 2, 5, 3]));
        Assert.Equal(MastermindGuessOutcome.Solved, game.Guess([2, 0, 5, 1]));

        Assert.True(game.IsSolved);
        Assert.True(game.IsOver);
        Assert.Equal(2, game.Guesses);
        Assert.Equal(new MastermindFeedback(1, 2), game.Rows[0].Feedback);
    }

    [Fact]
    public void The_code_stays_hidden_until_the_game_is_over()
    {
        var game = MediumWithCode(2, 0, 5, 1);

        Assert.Null(game.RevealedCode);

        game.Guess([2, 0, 5, 1]);

        Assert.Equal(new[] { 2, 0, 5, 1 }, game.RevealedCode);
    }

    [Fact]
    public void Ten_wrong_guesses_end_the_game()
    {
        var game = MediumWithCode(2, 0, 5, 1);

        for (var i = 1; i < MastermindRules.MaxGuesses; i++)
        {
            Assert.Equal(MastermindGuessOutcome.Scored, game.Guess([3, 3, 3, 3]));
        }

        Assert.Equal(MastermindGuessOutcome.OutOfGuesses, game.Guess([3, 3, 3, 3]));
        Assert.True(game.IsOver);
        Assert.False(game.IsSolved);
        Assert.Equal(0, game.GuessesLeft);
        Assert.NotNull(game.RevealedCode);
        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess([2, 0, 5, 1]));
    }

    [Theory]
    [InlineData(new[] { 0, 1, 2 })]        // too short
    [InlineData(new[] { 0, 1, 2, 3, 4 })]  // too long
    [InlineData(new[] { 0, 1, 2, 6 })]     // no colour 6 on a 6-colour board
    [InlineData(new[] { 0, 1, 2, -1 })]
    public void A_guess_the_board_cant_take_is_ignored(int[] guess)
    {
        var game = MediumWithCode(2, 0, 5, 1);

        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess(guess));
        Assert.Empty(game.Rows);
    }

    [Fact]
    public void Easy_refuses_a_repeated_colour()
    {
        var game = new MastermindGame(new Random(3));
        game.Start(Difficulty.Easy);

        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess([1, 1, 2, 3]));
        Assert.NotEqual(MastermindGuessOutcome.Invalid, game.Guess([1, 0, 2, 3]));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Easy_codes_never_repeat_a_colour(int seed)
    {
        var game = new MastermindGame(new Random(seed));
        game.Start(Difficulty.Easy);

        // play it out to see the code
        for (var i = 0; i < MastermindRules.MaxGuesses && !game.IsOver; i++) game.Guess([0, 1, 2, 3]);
        var code = game.RevealedCode ?? [0, 1, 2, 3];

        Assert.Equal(MastermindRules.Easy.Pegs, code.Count);
        Assert.Equal(code.Count, code.Distinct().Count());
        Assert.All(code, colour => Assert.InRange(colour, 0, MastermindRules.Easy.Colours - 1));
    }

    [Fact]
    public void Hard_is_five_pegs_of_eight_colours()
    {
        var game = new MastermindGame(new Random(9));
        game.Start(Difficulty.Hard);

        Assert.Equal(5, game.Setup.Pegs);
        Assert.Equal(8, game.Setup.Colours);
        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess([0, 1, 2, 3]));
        Assert.NotEqual(MastermindGuessOutcome.Invalid, game.Guess([7, 7, 7, 7, 7]));
    }
}
