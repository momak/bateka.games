using bateka.games.Engine.Mastermind;
using bateka.games.Engine.Tests.TestDoubles;

namespace bateka.games.Engine.Tests.Mastermind;

public sealed class MastermindGameTests
{
    /// <summary>Classic (repeats allowed) takes one Next(6) per column, so the code is exactly these.</summary>
    private static MastermindGame ClassicWithCode(params int[] code)
    {
        var game = new MastermindGame(new FixedRandom(code));
        game.Start(MastermindRules.Classic);
        return game;
    }

    private static MastermindGame Started(MastermindSetup setup, int seed)
    {
        var game = new MastermindGame(new Random(seed));
        game.Start(setup);
        return game;
    }

    [Fact]
    public void The_right_guess_solves_it()
    {
        var game = ClassicWithCode(2, 0, 5, 1);

        Assert.Equal(MastermindGuessOutcome.Scored, game.Guess([0, 2, 5, 3]));
        Assert.Equal(MastermindGuessOutcome.Solved, game.Guess([2, 0, 5, 1]));

        Assert.True(game.IsSolved);
        Assert.True(game.IsOver);
        Assert.Equal(2, game.GuessesUsed);
        Assert.Equal(new MastermindFeedback(1, 2), game.Rows[0].Feedback);
    }

    [Fact]
    public void The_code_stays_hidden_until_the_game_is_over()
    {
        var game = ClassicWithCode(2, 0, 5, 1);

        Assert.Null(game.RevealedCode);

        game.Guess([2, 0, 5, 1]);

        Assert.Equal(new[] { 2, 0, 5, 1 }, game.RevealedCode);
    }

    [Fact]
    public void Running_out_of_guesses_ends_the_game()
    {
        var game = new MastermindGame(new FixedRandom(2, 0, 5, 1));
        game.Start(MastermindRules.Classic with { Guesses = 6 });

        for (var i = 1; i < 6; i++)
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
        var game = ClassicWithCode(2, 0, 5, 1);

        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess(guess));
        Assert.Empty(game.Rows);
    }

    [Fact]
    public void Without_repeats_a_repeated_colour_is_refused()
    {
        var game = Started(MastermindRules.Classic with { AllowRepeats = false }, seed: 3);

        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess([1, 1, 2, 3]));
        Assert.NotEqual(MastermindGuessOutcome.Invalid, game.Guess([1, 0, 2, 3]));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Without_repeats_the_code_never_repeats_a_colour(int seed)
    {
        var setup = new MastermindSetup(Columns: 6, Colours: 6, AllowRepeats: false, Guesses: 6);
        var game = Started(setup, seed);

        // play it out to see the code
        while (!game.IsOver) game.Guess([0, 1, 2, 3, 4, 5]);
        var code = game.RevealedCode!;

        Assert.Equal(6, code.Count);
        Assert.Equal(6, code.Distinct().Count());
        Assert.All(code, colour => Assert.InRange(colour, 0, 5));
    }

    [Fact]
    public void A_bigger_board_takes_longer_guesses()
    {
        var game = Started(new MastermindSetup(Columns: 5, Colours: 8, AllowRepeats: true, Guesses: 12), seed: 9);

        Assert.Equal(MastermindGuessOutcome.Invalid, game.Guess([0, 1, 2, 3]));
        Assert.NotEqual(MastermindGuessOutcome.Invalid, game.Guess([7, 7, 7, 7, 7]));
        Assert.Equal(11, game.GuessesLeft);
    }

    [Theory]
    [InlineData(4, 6, true, 10, true)]    // classic
    [InlineData(3, 4, false, 6, true)]    // smallest
    [InlineData(6, 8, true, 12, true)]    // biggest
    [InlineData(6, 4, false, 10, false)]  // 6 different colours out of 4: impossible
    [InlineData(6, 4, true, 10, true)]    // …fine with repeats
    [InlineData(2, 6, true, 10, false)]   // too few columns
    [InlineData(4, 9, true, 10, false)]   // too many colours
    [InlineData(4, 6, true, 5, false)]    // too few guesses
    public void Only_playable_boards_can_start(int columns, int colours, bool repeats, int guesses, bool valid)
    {
        var setup = new MastermindSetup(columns, colours, repeats, guesses);

        Assert.Equal(valid, MastermindRules.IsValidSetup(setup));
        if (!valid) Assert.Throws<ArgumentOutOfRangeException>(() => new MastermindGame(new Random(1)).Start(setup));
    }
}
