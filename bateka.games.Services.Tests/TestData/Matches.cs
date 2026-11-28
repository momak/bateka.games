using bateka.games.Domain.Entities;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Matches;

namespace bateka.games.Services.Tests.TestData;

/// <summary>Matches between "ana" (created it, moves first) and "me", at chosen minutes.</summary>
public static class Matches
{
    public const string Ana = "ana";
    public const string Me = "me";

    public static Match Waiting(int createdMinute = 0) => new()
    {
        Id = Guid.NewGuid(),
        Game = GameKind.TicTacToe,
        Status = MatchStatus.Waiting,
        CreatorId = Ana,
        MoveSeconds = 60,
        CreatedAt = Players.At(createdMinute)
    };

    /// <summary>Started at minute 0; <paramref name="moves"/> already played; deadline one minute after <paramref name="lastMoveMinute"/>.</summary>
    public static Match Playing(int[] moves, int lastMoveMinute = 0)
    {
        var match = Waiting();
        match.OpponentId = Me;
        match.FirstPlayerId = Ana;
        match.Status = MatchStatus.Playing;
        match.StartedAt = Players.At(0);
        match.Moves = [.. moves];
        match.MoveDeadline = Players.At(lastMoveMinute + 1);
        return match;
    }
}
