using bateka.games.Services.Abstractions;
using bateka.games.Shared.Contracts.Players;

namespace bateka.games.Services.Players;

/// <summary>Who you can invite to a match: everyone who has played here, except you.</summary>
public sealed class PlayerService(IPlayerRepository players, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<PlayerDto>> ListOthersAsync(CancellationToken ct)
    {
        var me = currentUser.UserId;
        var known = await players.ListKnownAsync(ct);
        return known.Where(name => name != me).Select(name => new PlayerDto(name)).ToList();
    }
}
