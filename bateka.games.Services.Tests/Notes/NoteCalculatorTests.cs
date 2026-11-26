using bateka.games.Services.Notes;
using bateka.games.Shared.Contracts.Games;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Notes;

public sealed class NoteCalculatorTests
{
    [Fact]
    public void Someone_passing_you_after_you_last_looked_is_a_note()
    {
        var results = new[] { P.Snake("me", 200, 1), P.Snake("ana", 150, 2), P.Snake("ana", 260, 10) };

        var note = Assert.Single(NoteCalculator.For("me", results, since: P.At(5)));

        Assert.Equal(GameKind.Snake, note.Game);
        Assert.Equal("ana", note.Player);
        Assert.Equal(260, note.TheirValue);
        Assert.Equal(200, note.YourValue);
        Assert.Equal(P.At(10), note.PassedAt);
    }

    [Fact]
    public void A_pass_you_already_saw_is_not_a_note()
    {
        var results = new[] { P.Snake("me", 200, 1), P.Snake("ana", 260, 3) };

        Assert.Empty(NoteCalculator.For("me", results, since: P.At(5)));
    }

    [Fact]
    public void Being_behind_from_before_you_joined_is_not_news()
    {
        var results = new[] { P.Snake("ana", 500, 1), P.Snake("me", 100, 10) };

        Assert.Empty(NoteCalculator.For("me", results, since: P.At(0)));
    }

    [Fact]
    public void Only_boards_you_play_on_count()
    {
        var results = new[]
        {
            P.Memory("me", 6, moves: 10, seconds: 30, minute: 1),
            P.Memory("ana", 8, moves: 12, seconds: 30, minute: 5), // another size
            P.Memory("ana", 6, moves: 8, seconds: 30, minute: 6)
        };

        var note = Assert.Single(NoteCalculator.For("me", results, since: P.At(2)));

        Assert.Equal(6, note.Pairs);
        Assert.Equal(8, note.TheirValue);
    }

    [Fact]
    public void No_results_means_no_notes()
    {
        Assert.Empty(NoteCalculator.For("me", [P.Snake("ana", 100, 1)], since: P.At(0)));
    }
}
