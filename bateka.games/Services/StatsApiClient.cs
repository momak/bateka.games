using bateka.games.Shared.Contracts.Stats;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>My stats in every game.</summary>
public sealed class StatsApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    public Task<MyStatsDto> GetMineAsync(CancellationToken ct = default) =>
        GetAsync<MyStatsDto>(StatsRoutes.Mine, ct);
}
