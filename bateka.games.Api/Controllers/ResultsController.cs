using bateka.games.Services.GameResults;
using bateka.games.Shared.Contracts.Results;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>The current user's finished games. Bodies are validated by the global ValidationFilter.</summary>
[ApiController]
public sealed class ResultsController(GameResultService results) : ControllerBase
{
    /// <summary>Records one finished game; answers with "personal best?" and the updated stats.</summary>
    [HttpPost(ResultRoutes.Base)]
    public async Task<ActionResult<RecordGameResultResponse>> Record(RecordGameResultRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await results.RecordAsync(request, ct));
}
