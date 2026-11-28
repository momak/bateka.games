using bateka.games.Shared.Contracts.Players;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

public sealed class PlayersApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    /// <summary>Who I can invite: everyone who has played here, except me.</summary>
    public Task<List<PlayerDto>> ListAsync(CancellationToken ct = default) =>
        GetAsync<List<PlayerDto>>(PlayerRoutes.Base, ct);

    public Task<PlayerDto> GetMeAsync(CancellationToken ct = default) =>
        GetAsync<PlayerDto>(PlayerRoutes.Me, ct);
}
