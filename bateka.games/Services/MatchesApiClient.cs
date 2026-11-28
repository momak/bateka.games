using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Json;
using bateka.games.Shared.Routing;
using Bateka.UI.Services;

namespace bateka.games.Services;

/// <summary>Online matches: every action goes through here; live updates come from <see cref="MatchHubClient"/>.</summary>
public sealed class MatchesApiClient(HttpClient http) : ApiClientBase(http, GamesJson.Options)
{
    public Task<MatchDto> CreateAsync(CreateMatchRequest request, CancellationToken ct = default) =>
        PostAsync<MatchDto>(MatchRoutes.Base, request, ct);

    public Task<MatchDto> GetAsync(Guid id, CancellationToken ct = default) =>
        GetAsync<MatchDto>(MatchRoutes.For(id), ct);

    /// <summary>Waiting and playing matches, then the latest ended ones.</summary>
    public Task<List<MatchDto>> ListMineAsync(CancellationToken ct = default) =>
        GetAsync<List<MatchDto>>(MatchRoutes.Mine, ct);

    public Task<MatchDto> JoinAsync(Guid id, CancellationToken ct = default) =>
        PostAsync<MatchDto>(MatchRoutes.JoinFor(id), body: null, ct);

    public Task<MatchDto> MoveAsync(Guid id, int move, CancellationToken ct = default) =>
        PostAsync<MatchDto>(MatchRoutes.MovesFor(id), new MakeMoveRequest { Move = move }, ct);

    /// <summary>Gives up a game being played, or withdraws your own invite.</summary>
    public Task<MatchDto> ResignAsync(Guid id, CancellationToken ct = default) =>
        PostAsync<MatchDto>(MatchRoutes.ResignFor(id), body: null, ct);
}
