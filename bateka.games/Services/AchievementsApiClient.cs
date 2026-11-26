using bateka.games.Shared.Contracts.Achievements;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>Every achievement: mine, and who else has it.</summary>
public sealed class AchievementsApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    public Task<List<AchievementDto>> ListAsync(CancellationToken ct = default) =>
        GetAsync<List<AchievementDto>>(AchievementRoutes.Base, ct);
}
