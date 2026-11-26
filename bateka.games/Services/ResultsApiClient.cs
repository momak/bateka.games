using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>My finished games.</summary>
public sealed class ResultsApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    /// <summary>Records a finished game; answers with "personal best?" and my updated stats.</summary>
    public Task<RecordGameResultResponse> RecordAsync(RecordGameResultRequest request, CancellationToken ct = default) =>
        PostAsync<RecordGameResultResponse>(ResultRoutes.Base, request, ct);
}
