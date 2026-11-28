using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace bateka.games.Api.Hubs;

/// <summary>Sends a changed match to its group. A failed push is logged, never thrown: the move is already saved.</summary>
public sealed class MatchHubNotifier(IHubContext<MatchHub> hub, ILogger<MatchHubNotifier> logger) : IMatchNotifier
{
    public async Task MatchChangedAsync(MatchDto match, CancellationToken ct)
    {
        try
        {
            await hub.Clients.Group(MatchGroups.For(match.Id)).SendAsync(MatchHubContract.MatchUpdated, match, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Couldn't push match {MatchId} ({Status})", match.Id, match.Status);
        }
    }
}
