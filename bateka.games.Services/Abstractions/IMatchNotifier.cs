using bateka.games.Shared.Contracts.Matches;

namespace bateka.games.Services.Abstractions;

/// <summary>
/// Tells everyone watching a match that it changed (SignalR, in the Api). Called after the change is
/// saved; never fails the caller — a missed push is caught up by the client's next read.
/// </summary>
public interface IMatchNotifier
{
    Task MatchChangedAsync(MatchDto match, CancellationToken ct);
}
