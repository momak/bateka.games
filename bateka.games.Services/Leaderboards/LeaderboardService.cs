using bateka.games.Engine;
using bateka.games.Engine.Memory;
using bateka.games.Services.Abstractions;
using bateka.games.Services.Boards;
using bateka.games.Services.Results;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Leaderboards;

namespace bateka.games.Services.Leaderboards;

/// <summary>One game's all-time board, with the current user's row marked.</summary>
public sealed class LeaderboardService(IGameResultRepository results, ICurrentUser currentUser)
{
    private const string NeedsPairs = "Memory boards are per size: add pairs (3–8).";
    private const string NeedsDifficulty = "This board is per difficulty: add difficulty (easy, medium or hard).";
    private const string NoVariant = "Snake has a single board: no difficulty or pairs.";

    public async Task<ServiceResult<LeaderboardDto>> GetAsync(GameKind game, Difficulty? difficulty, int? pairs, CancellationToken ct)
    {
        if (BoardFor(game, difficulty, pairs) is not { } board) return ErrorFor(game);

        var onBoard = await results.ListForBoardAsync(board, ct);
        return LeaderboardCalculator.For(board, onBoard, currentUser.UserId);
    }

    /// <summary>The board those parameters name, or null when they don't fit the game.</summary>
    private static BoardKey? BoardFor(GameKind game, Difficulty? difficulty, int? pairs) => game switch
    {
        GameKind.Snake when difficulty is null && pairs is null => new(game, null, null),
        GameKind.Memory when difficulty is null && pairs is { } p && MemoryRules.IsValidSize(p) => new(game, null, p),
        GameKind.TicTacToe or GameKind.ConnectFour when pairs is null && difficulty is { } d && Enum.IsDefined(d) => new(game, d, null),
        _ => null
    };

    private static ServiceError ErrorFor(GameKind game) => ServiceError.Invalid(game switch
    {
        GameKind.Snake => NoVariant,
        GameKind.Memory => NeedsPairs,
        _ => NeedsDifficulty
    });
}
