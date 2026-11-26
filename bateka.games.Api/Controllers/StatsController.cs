using bateka.games.Services.GameResults;
using bateka.games.Shared.Contracts.Stats;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>The current user's stats, worked out from their results.</summary>
[ApiController]
public sealed class StatsController(GameResultService results) : ControllerBase
{
    [HttpGet(StatsRoutes.Mine)]
    public async Task<ActionResult<MyStatsDto>> Mine(CancellationToken ct) =>
        Ok(await results.GetMineAsync(ct));
}
