using bateka.games.Engine;
using bateka.games.Engine.Memory;
using bateka.games.Presentation;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Achievements;
using bateka.games.Shared.Contracts.Games;
using bateka.games.Shared.Contracts.Leaderboards;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Pages.Leaderboard;

/// <summary>
/// The all-time boards (pick a game, then its size or difficulty) and every achievement with who else
/// has it. Each board loads on its own, with a skeleton.
/// </summary>
public partial class Leaderboard : BusyComponentBase
{
    private const int OthersShown = 3;
    private const string DateFormat = "d MMM yyyy";
    private const string AchievementCssClass = "games-achievement";
    private const string LockedCssClass = "games-achievement--locked";

    private static readonly IReadOnlyList<Difficulty> Difficulties = [Difficulty.Easy, Difficulty.Medium, Difficulty.Hard];

    [Inject] private LeaderboardsApiClient Boards { get; set; } = default!;
    [Inject] private AchievementsApiClient Achievements { get; set; } = default!;

    private GameKind _game = GameKind.Snake;
    private Difficulty _difficulty = Difficulty.Hard;
    private int _pairs = MemoryRules.DefaultPairs;

    /// <summary>Null while loading.</summary>
    private LeaderboardDto? _board;
    private List<AchievementDto>? _achievements;

    /// <summary>Only the newest board request may fill the table (quick taps on the chips).</summary>
    private int _boardRequest;

    private int UnlockedCount => _achievements?.Count(a => a.IsUnlocked) ?? 0;

    protected override Task OnInitializedAsync() => Task.WhenAll(LoadBoardAsync(), LoadAchievementsAsync());

    private Task OnGameChangedAsync(GameKind game)
    {
        _game = game;
        return LoadBoardAsync();
    }

    private Task OnPairsChangedAsync(int pairs)
    {
        _pairs = pairs;
        return LoadBoardAsync();
    }

    private Task OnDifficultyChangedAsync(Difficulty difficulty)
    {
        _difficulty = difficulty;
        return LoadBoardAsync();
    }

    private async Task LoadBoardAsync()
    {
        var request = ++_boardRequest;
        _board = null;

        var game = _game;
        Difficulty? difficulty = GameNames.HasDifficulty(game) ? _difficulty : null;
        int? pairs = game == GameKind.Memory ? _pairs : null;

        var board = await LoadAsync(() => Boards.GetAsync(game, difficulty, pairs));
        if (request != _boardRequest) return;

        _board = board ?? new LeaderboardDto(game, difficulty, pairs, []);
    }

    private async Task LoadAchievementsAsync() =>
        _achievements = await LoadAsync(() => Achievements.ListAsync()) ?? [];

    private static string AchievementClass(AchievementDto achievement) =>
        achievement.IsUnlocked ? AchievementCssClass : $"{AchievementCssClass} {LockedCssClass}";

    private static string LocalDate(DateTime utc) => utc.ToLocalTime().ToString(DateFormat);

    /// <summary>"Also: ana, marko and 2 more" / "Nobody else yet".</summary>
    private static string OthersText(AchievementDto achievement)
    {
        var others = achievement.Others;
        if (others.Count == 0) return achievement.IsUnlocked ? "Nobody else yet" : "Nobody has it yet";

        var names = string.Join(", ", others.Take(OthersShown).Select(o => o.Player));
        var more = others.Count > OthersShown ? $" and {others.Count - OthersShown} more" : string.Empty;
        var prefix = achievement.IsUnlocked ? "Also" : "Unlocked by";
        return $"{prefix}: {names}{more}";
    }
}
