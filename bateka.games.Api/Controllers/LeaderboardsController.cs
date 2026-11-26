using bateka.games.Api.Results;
using bateka.games.Engine;
using bateka.games.Services.Leaderboards;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>All-time boards across every player.</summary>
[ApiController]
public sealed class LeaderboardsController(LeaderboardService leaderboards) : ControllerBase
{
    /// <summary>One board: Snake as is, Memory with <c>?pairs=</c>, Tic Tac Toe / Connect Four with <c>?difficulty=</c>.</summary>
    [HttpGet(LeaderboardRoutes.GameTemplate)]
    public async Task<IActionResult> Get(
        GameKind game,
        [FromQuery(Name = LeaderboardRoutes.DifficultyParameter)] Difficulty? difficulty,
        [FromQuery(Name = LeaderboardRoutes.PairsParameter)] int? pairs,
        CancellationToken ct) =>
        this.ToActionResult(await leaderboards.GetAsync(game, difficulty, pairs, ct));
}
