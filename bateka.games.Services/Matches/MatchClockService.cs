using bateka.games.Services.Abstractions;
using bateka.games.Shared.Matches;

namespace bateka.games.Services.Matches;

/// <summary>
/// One tick of the match clock: ends every match whose time is up and pushes it, so the waiting
/// player sees "you won on time" without anyone reloading. Each match is read and saved on its own,
/// so one conflict (a player moving in the same instant) doesn't hold up the others.
/// </summary>
public sealed class MatchClockService(IMatchRepository matches, IMatchNotifier notifier, TimeProvider clock)
{
    /// <summary>How many matches this tick ended.</summary>
    public async Task<int> TickAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var due = await matches.ListDueAsync(now, now - MatchLimits.InviteLifetime, ct);

        var ended = 0;
        foreach (var id in due)
        {
            var match = await matches.GetAsync(id, ct);

            // gone, or a player's own request ended it meanwhile
            if (match is null || !MatchTimeouts.Apply(match, now)) continue;

            // conflict: the other side saved first — the next tick sees their version
            if (!await matches.TrySaveChangesAsync(ct)) continue;

            await notifier.MatchChangedAsync(MatchMapper.ToDto(match), ct);
            ended++;
        }

        return ended;
    }
}
