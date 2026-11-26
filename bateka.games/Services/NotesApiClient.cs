using bateka.games.Shared.Contracts.Notes;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>My "Ana passed you" notes.</summary>
public sealed class NotesApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    public Task<List<NoteDto>> ListAsync(CancellationToken ct = default) =>
        GetAsync<List<NoteDto>>(NoteRoutes.Base, ct);

    /// <summary>Everything up to now is read.</summary>
    public Task MarkSeenAsync(CancellationToken ct = default) => PostAsync(NoteRoutes.Seen, ct);
}
