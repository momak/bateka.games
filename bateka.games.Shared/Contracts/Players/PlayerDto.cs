namespace bateka.games.Shared.Contracts.Players;

/// <summary>The signed-in player as the API sees them — the name matches use for "you".</summary>
public sealed record PlayerDto(string Username);
