using bateka.games.Services.Abstractions;
using bateka.games.Services.Players;
using bateka.games.Shared.Contracts.Players;
using bateka.games.Shared.Routing;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Controllers;

/// <summary>
/// The current player's name, exactly as the API compares it. The client needs it to tell "you"
/// from the opponent in a match; Bateka.UI's session user isn't there when running without the BFF.
/// </summary>
[ApiController]
public sealed class PlayersController(ICurrentUser currentUser, PlayerService players) : ControllerBase
{
    /// <summary>Who you can invite to a match.</summary>
    [HttpGet(PlayerRoutes.Base)]
    public async Task<ActionResult<IReadOnlyList<PlayerDto>>> List(CancellationToken ct) =>
        Ok(await players.ListOthersAsync(ct));

    [HttpGet(PlayerRoutes.Me)]
    public ActionResult<PlayerDto> Me() => Ok(new PlayerDto(currentUser.UserId));
}
