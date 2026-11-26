using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Leaderboards;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>The all-time boards.</summary>
public sealed class LeaderboardsApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    public Task<LeaderboardDto> GetAsync(GameKind game, Difficulty? difficulty, int? pairs, CancellationToken ct = default) =>
        GetAsync<LeaderboardDto>(LeaderboardRoutes.For(game, difficulty, pairs), ct);
}
