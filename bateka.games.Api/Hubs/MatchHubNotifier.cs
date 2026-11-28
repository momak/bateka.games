using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace bateka.games.Api.Hubs;

/// <summary>
/// Sends a changed match to its watchers (the match page) and to its players wherever they are
/// (their other pages: "Your matches", the invite pop-up). A failed push is logged, never thrown:
/// the change is already saved.
/// </summary>
public sealed class MatchHubNotifier(IHubContext<MatchHub> hub, ILogger<MatchHubNotifier> logger) : IMatchNotifier
{
    public async Task MatchChangedAsync(MatchDto match, CancellationToken ct)
    {
        try
        {
            await hub.Clients.Group(MatchGroups.For(match.Id)).SendAsync(MatchHubContract.MatchUpdated, match, ct);

            string?[] people = [match.Creator, match.Opponent, match.Invited];
            var users = people.OfType<string>().Distinct().ToList();
            await hub.Clients.Users(users).SendAsync(MatchHubContract.MyMatchChanged, match, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Couldn't push match {MatchId} ({Status})", match.Id, match.Status);
        }
    }
}
