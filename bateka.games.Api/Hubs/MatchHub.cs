using bateka.games.Api.Identity;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Matches;
using bateka.games.Shared.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace bateka.games.Api.Hubs;

/// <summary>
/// Live updates of online matches. It only lets a connection watch a match; moves and everything
/// else go through MatchesController, and MatchService pushes the result here
/// (<see cref="MatchHubNotifier"/>). The connection's user is the one ForwardedUserMiddleware put on
/// the request that opened it (Bateka.BFF forwards X-Bateka-User on the WebSocket upgrade too).
/// </summary>
public sealed class MatchHub(IMatchRepository matches) : Hub
{
    private const string NotFound = "That match doesn't exist (or isn't yours).";

    /// <summary>
    /// Adds this connection to the match's group. Read the match (GET) after this, not before, so no
    /// change can slip in between. Call again after a reconnect — groups don't survive one.
    /// </summary>
    [HubMethodName(MatchHubContract.Watch)]
    public async Task Watch(Guid matchId)
    {
        var userId = Context.GetHttpContext()?.GetRequiredUserId()
            ?? throw new HubException(NotFound);

        var match = await matches.GetAsync(matchId, Context.ConnectionAborted);
        if (match is null || !MatchPlayers.CanSee(match, userId)) throw new HubException(NotFound);

        await Groups.AddToGroupAsync(Context.ConnectionId, MatchGroups.For(matchId), Context.ConnectionAborted);
    }

    [HubMethodName(MatchHubContract.Unwatch)]
    public Task Unwatch(Guid matchId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, MatchGroups.For(matchId), Context.ConnectionAborted);
}
