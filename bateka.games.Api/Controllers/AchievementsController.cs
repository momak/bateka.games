using bateka.games.Services.Achievements;
using bateka.games.Shared.Contracts.Achievements;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>Every achievement: yours, and who else has it.</summary>
[ApiController]
public sealed class AchievementsController(AchievementService achievements) : ControllerBase
{
    [HttpGet(AchievementRoutes.Base)]
    public async Task<ActionResult<IReadOnlyList<AchievementDto>>> List(CancellationToken ct) =>
        Ok(await achievements.GetAsync(ct));
}
