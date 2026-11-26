using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Notes;

namespace bateka.games.Services.Notes;

/// <summary>The current user's "Ana passed you" notes, and marking them read.</summary>
public sealed class NoteService(
    IGameResultRepository results,
    IPlayerRepository players,
    ICurrentUser currentUser,
    TimeProvider clock)
{
    public async Task<IReadOnlyList<NoteDto>> GetAsync(CancellationToken ct)
    {
        var me = currentUser.UserId;
        var mine = await results.ListForUserAsync(me, ct);
        if (mine.Count == 0) return [];

        var games = mine.Select(r => r.Game).Distinct().ToList();
        var everyone = await results.ListForGamesAsync(games, ct);

        var player = await players.GetAsync(me, ct);
        var since = player?.NotesSeenAt ?? clock.GetUtcNow().UtcDateTime.AddDays(-NoteRules.FirstVisitDays);

        return NoteCalculator.For(me, everyone, since);
    }

    /// <summary>Everything up to now is read.</summary>
    public async Task MarkSeenAsync(CancellationToken ct)
    {
        var me = currentUser.UserId;
        var player = await players.GetAsync(me, ct);
        if (player is null)
        {
            player = new Player { UserId = me };
            players.Add(player);
        }

        player.NotesSeenAt = clock.GetUtcNow().UtcDateTime;
        await players.SaveChangesAsync(ct);
    }
}
