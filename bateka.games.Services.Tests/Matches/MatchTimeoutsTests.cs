using bateka.games.Services.Matches;
using bateka.games.Shared.Contracts.Matches;
using M = bateka.games.Services.Tests.TestData.Matches;
using P = bateka.games.Services.Tests.TestData.Players;

namespace bateka.games.Services.Tests.Matches;

public sealed class MatchTimeoutsTests
{
    [Fact]
    public void The_player_to_move_loses_when_their_time_is_up()
    {
        // one move played → "me" (second seat) is to move, deadline at minute 2
        var match = M.Playing([4], lastMoveMinute: 1);

        Assert.True(MatchTimeouts.Apply(match, P.At(3)));

        Assert.Equal(MatchStatus.Finished, match.Status);
        Assert.Equal(MatchEndReason.TimedOut, match.EndReason);
        Assert.Equal(M.Ana, match.WinnerId);
        Assert.Equal(P.At(2), match.FinishedAt);
        Assert.Null(match.MoveDeadline);
    }

    [Fact]
    public void Before_the_deadline_nothing_changes()
    {
        var match = M.Playing([4], lastMoveMinute: 1);

        Assert.False(MatchTimeouts.Apply(match, P.At(1)));
        Assert.Equal(MatchStatus.Playing, match.Status);
    }

    [Fact]
    public void An_invite_nobody_joins_expires_after_a_day()
    {
        var match = M.Waiting(createdMinute: 0);

        Assert.False(MatchTimeouts.Apply(match, P.At(23 * 60)));
        Assert.True(MatchTimeouts.Apply(match, P.At(24 * 60)));

        Assert.Equal(MatchStatus.Cancelled, match.Status);
        Assert.Equal(MatchEndReason.InviteExpired, match.EndReason);
    }

    [Fact]
    public void A_finished_match_is_left_alone()
    {
        var match = M.Playing([4], lastMoveMinute: 1);
        MatchEnding.Finish(match, M.Ana, MatchEndReason.Resigned, P.At(1));

        Assert.False(MatchTimeouts.Apply(match, P.At(100)));
        Assert.Equal(MatchEndReason.Resigned, match.EndReason);
    }
}
