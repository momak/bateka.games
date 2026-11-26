using bateka.games.Services.Notes;
using bateka.games.Shared.Contracts.Notes;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>"Ana passed your Snake best" — the current user's unread notes.</summary>
[ApiController]
public sealed class NotesController(NoteService notes) : ControllerBase
{
    [HttpGet(NoteRoutes.Base)]
    public async Task<ActionResult<IReadOnlyList<NoteDto>>> List(CancellationToken ct) =>
        Ok(await notes.GetAsync(ct));

    /// <summary>Everything up to now is read.</summary>
    [HttpPost(NoteRoutes.Seen)]
    public async Task<IActionResult> MarkSeen(CancellationToken ct)
    {
        await notes.MarkSeenAsync(ct);
        return NoContent();
    }
}
