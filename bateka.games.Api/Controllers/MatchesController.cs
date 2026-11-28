using bateka.games.Api.Results;
using bateka.games.Services.Matches;
using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>Online matches between two users. Bodies are validated by the global ValidationFilter.</summary>
[ApiController]
public sealed class MatchesController(MatchService matches) : ControllerBase
{
    /// <summary>A new match, waiting for someone to open its link (or for the invited player).</summary>
    [HttpPost(MatchRoutes.Base)]
    public async Task<IActionResult> Create(CreateMatchRequest request, CancellationToken ct) =>
        this.ToCreatedResult(await matches.CreateAsync(request, ct));

    /// <summary>Waiting and playing matches, then the latest ended ones.</summary>
    [HttpGet(MatchRoutes.Mine)]
    public async Task<ActionResult<IReadOnlyList<MatchDto>>> Mine(CancellationToken ct) =>
        Ok(await matches.ListMineAsync(ct));

    [HttpGet(MatchRoutes.ById)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct) =>
        this.ToActionResult(await matches.GetAsync(id, ct));

    [HttpPost(MatchRoutes.Join)]
    public async Task<IActionResult> Join(Guid id, CancellationToken ct) =>
        this.ToActionResult(await matches.JoinAsync(id, ct));

    [HttpPost(MatchRoutes.Moves)]
    public async Task<IActionResult> Move(Guid id, MakeMoveRequest request, CancellationToken ct) =>
        this.ToActionResult(await matches.MoveAsync(id, request, ct));

    /// <summary>After a finished game: a direct invite to the same opponent (or the one they already offered).</summary>
    [HttpPost(MatchRoutes.Rematch)]
    public async Task<IActionResult> Rematch(Guid id, CancellationToken ct) =>
        this.ToCreatedResult(await matches.RematchAsync(id, ct));

    /// <summary>Gives up a game being played, or withdraws your own invite.</summary>
    [HttpPost(MatchRoutes.Resign)]
    public async Task<IActionResult> Resign(Guid id, CancellationToken ct) =>
        this.ToActionResult(await matches.ResignAsync(id, ct));
}
