using bateka.games.Engine.Memory;

namespace bateka.games.Engine.Tests.Memory;

public sealed class MemoryGameTests
{
    private static MemoryGame Started(int pairs = 3)
    {
        var game = new MemoryGame(new Random(7));
        game.Start(pairs);
        return game;
    }

    private static (int First, int Second) APair(MemoryGame game) =>
        game.Cards
            .Select((card, index) => (card, index))
            .Where(x => !x.card.IsMatched)
            .GroupBy(x => x.card.Face)
            .Select(g => (g.First().index, g.Last().index))
            .First();

    private static (int First, int Second) TwoDifferent(MemoryGame game)
    {
        var first = game.Cards[0].Face;
        var other = game.Cards.Select((card, index) => (card, index)).First(x => x.card.Face != first).index;
        return (0, other);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(8)]
    public void Start_deals_every_face_exactly_twice(int pairs)
    {
        var game = Started(pairs);

        Assert.Equal(pairs * 2, game.Cards.Count);
        Assert.All(game.Cards.GroupBy(c => c.Face), g => Assert.Equal(2, g.Count()));
        Assert.All(game.Cards, card => Assert.False(card.IsShown));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(9)]
    public void Start_refuses_a_size_outside_three_to_eight(int pairs)
    {
        var game = new MemoryGame(new Random(7));

        Assert.Throws<ArgumentOutOfRangeException>(() => game.Start(pairs));
    }

    [Fact]
    public void Tapping_the_same_card_twice_only_turns_it_once()
    {
        var game = Started();

        Assert.Equal(MemoryFlipOutcome.FirstCard, game.Flip(0));
        Assert.Equal(MemoryFlipOutcome.Ignored, game.Flip(0));
        Assert.Equal(0, game.Moves);
    }

    [Fact]
    public void The_second_card_counts_a_move_and_blocks_taps_until_resolved()
    {
        var game = Started();
        var (first, second) = TwoDifferent(game);
        var third = Enumerable.Range(0, game.Cards.Count).First(i => i != first && i != second);

        game.Flip(first);

        Assert.Equal(MemoryFlipOutcome.SecondCard, game.Flip(second));
        Assert.Equal(1, game.Moves);
        Assert.True(game.IsCheckingPair);
        Assert.Equal(MemoryFlipOutcome.Ignored, game.Flip(third));
    }

    [Fact]
    public void A_mismatch_turns_both_cards_back()
    {
        var game = Started();
        var (first, second) = TwoDifferent(game);
        game.Flip(first);
        game.Flip(second);

        Assert.Equal(MemoryPairOutcome.Mismatch, game.ResolvePair());
        Assert.False(game.Cards[first].IsShown);
        Assert.False(game.Cards[second].IsShown);
        Assert.False(game.IsCheckingPair);
    }

    [Fact]
    public void A_match_keeps_both_cards_shown()
    {
        var game = Started();
        var (first, second) = APair(game);
        game.Flip(first);
        game.Flip(second);

        Assert.Equal(MemoryPairOutcome.Match, game.ResolvePair());
        Assert.True(game.Cards[first].IsMatched);
        Assert.True(game.Cards[second].IsMatched);
        Assert.Equal(1, game.MatchesFound);
    }

    [Fact]
    public void Matching_the_last_pair_wins()
    {
        var game = Started(3);
        var outcome = MemoryPairOutcome.None;

        for (var i = 0; i < 3; i++)
        {
            var (first, second) = APair(game);
            game.Flip(first);
            game.Flip(second);
            outcome = game.ResolvePair();
        }

        Assert.Equal(MemoryPairOutcome.Won, outcome);
        Assert.True(game.IsWon);
        Assert.Equal(3, game.Moves);
        Assert.Equal(MemoryFlipOutcome.Ignored, game.Flip(0));
    }

    [Fact]
    public void Resolving_without_two_cards_does_nothing()
    {
        var game = Started();
        game.Flip(0);

        Assert.Equal(MemoryPairOutcome.None, game.ResolvePair());
        Assert.True(game.Cards[0].IsFaceUp);
    }

    [Fact]
    public void Every_size_has_a_layout_that_fits_all_its_cards()
    {
        Assert.All(MemoryRules.Sizes, pairs =>
        {
            var layout = MemoryRules.LayoutFor(pairs);
            Assert.Equal(pairs * 2, layout.Rows * layout.Columns);
        });
    }
}
