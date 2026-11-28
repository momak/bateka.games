using bateka.games.Services.Matches;
using M = bateka.games.Services.Tests.TestData.Matches;

namespace bateka.games.Services.Tests.Matches;

public sealed class MatchPlayersTests
{
    [Fact]
    public void Turns_follow_the_first_seat()
    {
        Assert.Equal(M.Ana, MatchPlayers.ToMove(M.Playing([])));
        Assert.Equal(M.Me, MatchPlayers.ToMove(M.Playing([4])));
        Assert.Equal(M.Ana, MatchPlayers.ToMove(M.Playing([4, 0])));
    }

    [Fact]
    public void Nobody_is_to_move_while_waiting()
    {
        Assert.Null(MatchPlayers.ToMove(M.Waiting()));
    }

    [Fact]
    public void The_other_player_is_the_other_seat()
    {
        var match = M.Playing([]);

        Assert.Equal(M.Me, MatchPlayers.Other(match, M.Ana));
        Assert.Equal(M.Ana, MatchPlayers.Other(match, M.Me));
        Assert.False(MatchPlayers.IsPlayer(match, "bob"));
    }

    [Fact]
    public void An_open_invite_is_visible_to_anyone_with_the_link()
    {
        Assert.True(MatchPlayers.CanSee(M.Waiting(), "bob"));
    }

    [Fact]
    public void A_direct_invite_is_visible_only_to_its_two_people()
    {
        var invite = M.DirectInvite(M.Me);

        Assert.True(MatchPlayers.CanSee(invite, M.Me));
        Assert.True(MatchPlayers.CanSee(invite, M.Ana));
        Assert.False(MatchPlayers.CanSee(invite, "bob"));
    }

    [Fact]
    public void A_started_match_is_visible_only_to_its_players()
    {
        var match = M.Playing([4]);

        Assert.True(MatchPlayers.CanSee(match, M.Me));
        Assert.False(MatchPlayers.CanSee(match, "bob"));
    }
}
