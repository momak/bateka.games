using bateka.games.Data;
using bateka.games.Domain.Entities;
using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Matches;
using Microsoft.EntityFrameworkCore;

namespace bateka.games.Repositories;

public sealed class MatchRepository(GamesDbContext db) : IMatchRepository
{
    public Task<Match?> GetAsync(Guid id, CancellationToken ct) =>
        db.Matches.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Match>> ListForUserAsync(string userId, int recentFinished, CancellationToken ct)
    {
        // a direct invite to me counts as mine too
        var mine = db.Matches.Where(x => x.CreatorId == userId || x.OpponentId == userId || x.InvitedId == userId);

        var open = await mine
            .Where(x => x.Status == MatchStatus.Waiting || x.Status == MatchStatus.Playing)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        var ended = await mine
            .Where(x => x.Status == MatchStatus.Finished || x.Status == MatchStatus.Cancelled)
            .OrderByDescending(x => x.FinishedAt)
            .Take(recentFinished)
            .ToListAsync(ct);

        return [.. open, .. ended];
    }

    // uses the (Status, MoveDeadline) and (Status, CreatedAt) indexes
    public async Task<IReadOnlyList<Guid>> ListDueAsync(DateTime now, DateTime invitesCreatedBefore, CancellationToken ct) =>
        await db.Matches.AsNoTracking()
            .Where(x => (x.Status == MatchStatus.Playing && x.MoveDeadline <= now)
                     || (x.Status == MatchStatus.Waiting && x.CreatedAt <= invitesCreatedBefore))
            .Select(x => x.Id)
            .ToListAsync(ct);

    public void Add(Match match) => db.Matches.Add(match);

    public async Task<bool> TrySaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // the other player (or the clock) got there first; forget our version of the row
            db.ChangeTracker.Clear();
            return false;
        }
    }
}
