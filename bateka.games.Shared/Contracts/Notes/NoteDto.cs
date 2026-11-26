using bateka.games.Engine;
using bateka.games.Shared.Contracts.Games;

namespace bateka.games.Shared.Contracts.Notes;

/// <summary>
/// Someone went ahead of you on a board since you last looked. The values are the board's main
/// number (see LeaderboardRowDto.Value): theirs now and yours now.
/// </summary>
public sealed record NoteDto(
    GameKind Game,
    Difficulty? Difficulty,
    int? Pairs,
    string Player,
    int TheirValue,
    int YourValue,
    DateTime PassedAt);
