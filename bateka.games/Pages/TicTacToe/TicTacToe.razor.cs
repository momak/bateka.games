using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace bateka.games.Pages.TicTacToe;

public partial class TicTacToe : ComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    // Board
    public CellState[] Board { get; private set; } = new CellState[9];
    public TicTacToeStats Stats { get; private set; } = new();

    private Difficulty _difficulty = Difficulty.Hard;
    private bool _gameOver = false;
    private bool _isAiThinking = false;
    private int _movesThisGame = 0;
    private GameResult _result = GameResult.None;
    private int[]? _winningLine;

    private static readonly int[][] WinningLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // cols
        [0, 4, 8], [2, 4, 6]              // diagonals
    ];

    // ── Computed UI ──────────────────────────────────────────

    public string StatusMessage => _result switch
    {
        GameResult.PlayerWin => "🎉 You win!",
        GameResult.AiWin => "🤖 AI wins!",
        GameResult.Draw => "🤝 It's a draw!",
        _ => _isAiThinking ? "🤔 AI is thinking..." : "Your turn — play X"
    };

    public Color StatusColor => _result switch
    {
        GameResult.PlayerWin => Color.Success,
        GameResult.AiWin => Color.Error,
        GameResult.Draw => Color.Warning,
        _ => Color.Default
    };

    public string GetCellClass(int index)
    {
        var classes = new List<string>();
        if (_winningLine?.Contains(index) == true)
            classes.Add("cell--winner");
        if (Board[index] != CellState.Empty)
            classes.Add("cell--filled");
        return string.Join(" ", classes);
    }

    // ── Lifecycle ────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        await LoadStats();
        ResetBoard();
    }

    // ── Player Move ──────────────────────────────────────────

    public async Task OnCellClick(int index)
    {
        if (Board[index] != CellState.Empty || _gameOver || _isAiThinking)
            return;

        Board[index] = CellState.X;
        _movesThisGame++;

        if (CheckResult()) return;

        _isAiThinking = true;
        StateHasChanged();

        // Small delay so the UI updates before AI calculates
        await Task.Delay(_difficulty == Difficulty.Hard ? 400 : 300);

        MakeAiMove();
        CheckResult();

        _isAiThinking = false;
    }

    // ── AI Move ──────────────────────────────────────────────

    private void MakeAiMove()
    {
        var move = _difficulty switch
        {
            Difficulty.Easy => GetEasyMove(),
            Difficulty.Medium => GetMediumMove(),
            Difficulty.Hard => GetBestMove(),
            _ => GetBestMove()
        };

        if (move >= 0)
        {
            Board[move] = CellState.O;
            _movesThisGame++;  
        }
    }

    private int GetEasyMove()
    {
        // Pure random
        var empty = EmptyCells();
        return empty.Count == 0 ? -1 : empty[Random.Shared.Next(empty.Count)];
    }

    private int GetMediumMove()
    {
        // 50% chance of best move, otherwise random
        return Random.Shared.Next(2) == 0 ? GetBestMove() : GetEasyMove();
    }

    private int GetBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (Board[i] != CellState.Empty) continue;
            Board[i] = CellState.O;
            int score = Minimax(Board, 0, false, int.MinValue, int.MaxValue);
            Board[i] = CellState.Empty;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = i;
            }
        }

        return bestMove;
    }

    private int Minimax(CellState[] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        var winner = GetWinner(board);
        if (winner == CellState.O) return 10 - depth;
        if (winner == CellState.X) return depth - 10;
        if (EmptyCells(board).Count == 0) return 0;

        if (isMaximizing)
        {
            int best = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != CellState.Empty) continue;
                board[i] = CellState.O;
                best = Math.Max(best, Minimax(board, depth + 1, false, alpha, beta));
                board[i] = CellState.Empty;
                alpha = Math.Max(alpha, best);
                if (beta <= alpha) break;
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != CellState.Empty) continue;
                board[i] = CellState.X;
                best = Math.Min(best, Minimax(board, depth + 1, true, alpha, beta));
                board[i] = CellState.Empty;
                beta = Math.Min(beta, best);
                if (beta <= alpha) break;
            }
            return best;
        }
    }

    // ── Game State ───────────────────────────────────────────

    private bool CheckResult()
    {
        var winner = GetWinner(Board);

        if (winner == CellState.X)
        {
            _result = GameResult.PlayerWin;
            _winningLine = GetWinningLine(Board);
            _gameOver = true;
            Stats.Wins++;
            Stats.TotalMoves += _movesThisGame;
            _ = SaveStats();
            return true;
        }

        if (winner == CellState.O)
        {
            _result = GameResult.AiWin;
            _winningLine = GetWinningLine(Board);
            _gameOver = true;
            Stats.Losses++;
            Stats.TotalMoves += _movesThisGame;
            _ = SaveStats();
            return true;
        }

        if (EmptyCells().Count == 0)
        {
            _result = GameResult.Draw;
            _gameOver = true;
            Stats.Draws++;
            Stats.TotalMoves += _movesThisGame; 
            _ = SaveStats();
            return true;
        }

        return false;
    }

    private static CellState GetWinner(CellState[] board)
    {
        foreach (var line in WinningLines)
        {
            if (board[line[0]] != CellState.Empty &&
                board[line[0]] == board[line[1]] &&
                board[line[1]] == board[line[2]])
                return board[line[0]];
        }
        return CellState.Empty;
    }

    private static int[]? GetWinningLine(CellState[] board)
    {
        foreach (var line in WinningLines)
        {
            if (board[line[0]] != CellState.Empty &&
                board[line[0]] == board[line[1]] &&
                board[line[1]] == board[line[2]])
                return line;
        }
        return null;
    }

    private List<int> EmptyCells() => EmptyCells(Board);

    private static List<int> EmptyCells(CellState[] board) =>
        Enumerable.Range(0, 9).Where(i => board[i] == CellState.Empty).ToList();

    public void ResetGame()
    {
        ResetBoard();
        StateHasChanged();
    }

    private void ResetBoard()
    {
        Board = new CellState[9];
        _gameOver = false;
        _isAiThinking = false;
        _result = GameResult.None;
        _winningLine = null;
        _movesThisGame = 0;
    }

    public async Task ResetStats()
    {
        Stats = new TicTacToeStats();
        await SaveStats();
    }

    // ── Persistence ──────────────────────────────────────────

    private async Task SaveStats()
    {
        await JS.InvokeVoidAsync("localStorage.setItem", "ttt_stats",
            System.Text.Json.JsonSerializer.Serialize(Stats));
    }

    private async Task LoadStats()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", "ttt_stats");
            if (!string.IsNullOrEmpty(json))
                Stats = System.Text.Json.JsonSerializer.Deserialize<TicTacToeStats>(json) ?? new();
        }
        catch
        {
            Stats = new TicTacToeStats();
        }
    }
}