using bateka.games.Engine;
using bateka.games.Engine.TicTacToe;
using bateka.games.Stats;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace bateka.games.Pages.TicTacToe;

/// <summary>Tic Tac Toe's page: you are X against the computer. The rules and AI are in <see cref="TicTacToeGame"/>.</summary>
public partial class TicTacToe : ComponentBase
{
    /// <summary>A short pause before the computer answers, so its move doesn't look instant.</summary>
    private const int ThinkingMs = 300;
    private const int HardThinkingMs = 400;

    [Inject] private LocalStatsStore StatsStore { get; set; } = default!;

    private readonly TicTacToeGame _game = new(new TicTacToeAi(Random.Shared));

    private Difficulty _difficulty = Difficulty.Hard;
    private bool _isAiThinking;

    private TicTacToeStats Stats { get; set; } = new();

    private IReadOnlyList<CellState> Board => _game.Board;

    private string StatusMessage => _game.Result switch
    {
        TicTacToeResult.PlayerWin => "🎉 You win!",
        TicTacToeResult.AiWin => "🤖 AI wins!",
        TicTacToeResult.Draw => "🤝 It's a draw!",
        _ => _isAiThinking ? "🤔 AI is thinking..." : "Your turn — play X"
    };

    private Color StatusColor => _game.Result switch
    {
        TicTacToeResult.PlayerWin => Color.Success,
        TicTacToeResult.AiWin => Color.Error,
        TicTacToeResult.Draw => Color.Warning,
        _ => Color.Default
    };

    private string GetCellClass(int index)
    {
        var classes = new List<string>();
        if (_game.WinningLine?.Contains(index) == true) classes.Add("cell--winner");
        if (Board[index] != CellState.Empty) classes.Add("cell--filled");
        return string.Join(" ", classes);
    }

    private bool IsCellDisabled(int index) =>
        Board[index] != CellState.Empty || _game.IsOver || _isAiThinking;

    protected override async Task OnInitializedAsync()
    {
        Stats = await StatsStore.LoadAsync<TicTacToeStats>(StatsKeys.TicTacToe);
    }

    private async Task OnCellClickAsync(int index)
    {
        if (_isAiThinking || !_game.PlayerMove(index)) return;

        if (!_game.IsOver)
        {
            _isAiThinking = true;
            StateHasChanged();

            await Task.Delay(_difficulty == Difficulty.Hard ? HardThinkingMs : ThinkingMs);

            _game.AiMove(_difficulty);
            _isAiThinking = false;
        }

        if (_game.IsOver)
        {
            Stats.Record(_game.Result, _game.Moves);
            await StatsStore.SaveAsync(StatsKeys.TicTacToe, Stats);
        }
    }

    private void ResetGame()
    {
        _game.Reset();
        _isAiThinking = false;
    }

    private async Task ResetStatsAsync()
    {
        Stats = new TicTacToeStats();
        await StatsStore.SaveAsync(StatsKeys.TicTacToe, Stats);
    }
}
