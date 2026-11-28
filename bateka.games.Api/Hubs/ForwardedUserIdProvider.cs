using bateka.games.Api.Identity;
using Microsoft.AspNetCore.SignalR;

namespace bateka.games.Api.Hubs;

/// <summary>
/// A SignalR connection's user = the Bateka user ForwardedUserMiddleware put on the request that
/// opened it (X-Bateka-User from Bateka.BFF). Lets the server push to a user on any page.
/// </summary>
public sealed class ForwardedUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.GetHttpContext()?.GetRequiredUserId();
}
