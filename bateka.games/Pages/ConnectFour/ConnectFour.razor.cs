using bateka.games.Engine;
using bateka.games.Engine.ConnectFour;
using bateka.games.Presentation;
using bateka.games.Services;
using bateka.games.Shared.Contracts.Stats;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace bateka.games.Pages.ConnectFour;

/// <summary>
/// Connect Four's page: you (Red) against the computer or a second player on the same device. The
/// rules and AI are in <see cref="ConnectFourGame"/>; only games against the computer are counted.
/// </summary>
public partial class ConnectFour : BusyComponentBase
{
    /// <summary>A short pause before the computer drops its disc.</summary>
    private const int AiThinkingMs = 400;

    private const string RedClass = "red";
    private const string YellowClass = "yellow";

    [Inject] private StatsApiClient StatsApi { get; set; } = default!;
    [Inject] private ResultsApiClient ResultsApi { get; set; } = default!;

    private readonly ConnectFourGame _game = new(new ConnectFourAi(Random.Shared));

    private ConnectFourMode _mode = ConnectFourMode.VersusAi;
    private Difficulty _difficulty = Difficulty.Hard;
    private bool _playing;
    private int? _hoverColumn;

    /// <summary>Null while loading (the stats bar shows a skeleton).</summary>
    private ConnectFourStatsDto? Stats { get; set; }

    private static int Rows => ConnectFourBoard.Rows;

    private static int Columns => ConnectFourBoard.Columns;

    private string CurrentColorClass => _game.CurrentDisc == Disc.Red ? RedClass : YellowClass;

    private string StatusMessage
    {
        get
        {
            var versusAi = _game.Mode == ConnectFourMode.VersusAi;
            return _game.Result switch
            {
                ConnectFourResult.Draw => "🤝 It's a draw!",
                ConnectFourResult.RedWins => versusAi ? "🎉 You win!" : "🎉 Player 1 wins!",
                ConnectFourResult.YellowWins => versusAi ? "🤖 AI wins!" : "🎉 Player 2 wins!",
                _ when versusAi => _game.IsAiTurn ? "AI is thinking..." : "Your turn",
                _ => _game.CurrentDisc == Disc.Red ? "Player 1's turn" : "Player 2's turn"
            };
        }
    }

    private Color StatusColor => _game.Result switch
    {
        ConnectFourResult.Draw => Color.Warning,
        ConnectFourResult.RedWins => Color.Success,
        ConnectFourResult.YellowWins => Color.Error,
        _ => Color.Default
    };

    private static string GetDiscClass(Disc disc, bool isWinner) => disc switch
    {
        Disc.Red => $"disc--{RedClass}{(isWinner ? " disc--winner" : "")}",
        Disc.Yellow => $"disc--{YellowClass}{(isWinner ? " disc--winner" : "")}",
        _ => "disc--empty"
    };

    protected override async Task OnInitializedAsync()
    {
        var mine = await LoadAsync(() => StatsApi.GetMineAsync());
        Stats = mine?.ConnectFour ?? ConnectFourStatsDto.Empty;
    }

    private void StartGame()
    {
        _game.Start(_mode, _difficulty);
        _hoverColumn = null;
        _playing = true;
    }

    private void BackToSetup() => _playing = false;

    private async Task OnColumnClickAsync(int column)
    {
        if (!_game.Drop(column)) return;

        if (_game.IsAiTurn)
        {
            StateHasChanged();
            await Task.Delay(AiThinkingMs);
            _game.AiMove();
        }

        if (_game.IsOver && _game.Mode == ConnectFourMode.VersusAi)
        {
            await RunAsync(async () =>
            {
                var response = await ResultsApi.RecordAsync(ResultRequests.ForConnectFour(_game));
                Stats = response.Stats.ConnectFour;
            });
        }
    }
}
