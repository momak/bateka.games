using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace bateka.games.Pages.ConnectFour;

public enum GameMode { SinglePlayer, TwoPlayer }
public enum ConnectFourDifficulty { Easy, Medium, Hard }

public class ConnectFourStats
{
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestWinStreak { get; set; }
}

public partial class ConnectFour : ComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    public const int Rows = 6;
    public const int Cols = 7;

    // State
    public ConnectFourStats Stats { get; private set; } = new();
    private int[,] _board = new int[Rows, Cols];
    private int _currentPlayer = 1; // 1 = human / P1, 2 = AI / P2
    private bool _gameStarted = false;
    private bool _gameOver = false;
    private GameMode _gameMode = GameMode.SinglePlayer;
    private ConnectFourDifficulty _difficulty = ConnectFourDifficulty.Hard;
    private HashSet<(int, int)> _winningCells = [];
    private int? _hoverCol = null;

    // ── Computed UI ──────────────────────────────────────────

    public string StatusMessage
    {
        get
        {
            if (_gameOver)
            {
                if (!_winningCells.Any()) return "🤝 It's a draw!";
                if (_gameMode == GameMode.SinglePlayer)
                    return _currentPlayer == 1 ? "🎉 You win!" : "🤖 AI wins!";
                return $"🎉 Player {_currentPlayer} wins!";
            }

            if (_gameMode == GameMode.SinglePlayer)
                return _currentPlayer == 1 ? "Your turn" : "AI is thinking...";

            return $"Player {_currentPlayer}'s turn";
        }
    }

    public Color StatusColor => _gameOver
        ? (!_winningCells.Any() ? Color.Warning
            : (_currentPlayer == 1 ? Color.Success : Color.Error))
        : Color.Default;

    public string GetDiscClass(int cell, bool isWinner) =>
        cell == 0 ? "disc--empty"
        : cell == 1 ? $"disc--red{(isWinner ? " disc--winner" : "")}"
        : $"disc--yellow{(isWinner ? " disc--winner" : "")}";

    // ── Lifecycle ────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        await LoadStats();
    }

    // ── Game Control ─────────────────────────────────────────

    public void StartGame()
    {
        _board = new int[Rows, Cols];
        _currentPlayer = 1;
        _gameOver = false;
        _winningCells = [];
        _hoverCol = null;
        _gameStarted = true;
        StateHasChanged();
    }

    public void BackToSetup()
    {
        _gameStarted = false;
        _gameOver = false;
    }

    // ── Player Move ──────────────────────────────────────────

    public async Task OnColumnClick(int col)
    {
        if (_gameOver) return;
        if (_gameMode == GameMode.SinglePlayer && _currentPlayer == 2) return;
        if (_board[0, col] != 0) return; // column full

        if (!DropDisc(col, _currentPlayer)) return;

        if (CheckGameOver()) return;

        _currentPlayer = _currentPlayer == 1 ? 2 : 1;
        StateHasChanged();

        if (_gameMode == GameMode.SinglePlayer && _currentPlayer == 2)
        {
            await Task.Delay(400);
            MakeAiMove();

            if (!CheckGameOver())
                _currentPlayer = 1;

            StateHasChanged();
        }
    }

    private bool DropDisc(int col, int player)
    {
        for (int r = Rows - 1; r >= 0; r--)
        {
            if (_board[r, col] == 0)
            {
                _board[r, col] = player;
                return true;
            }
        }
        return false; // column full
    }

    // ── AI ───────────────────────────────────────────────────

    private void MakeAiMove()
    {
        int col = _difficulty switch
        {
            ConnectFourDifficulty.Easy => GetRandomMove(),
            ConnectFourDifficulty.Medium => GetMediumMove(),
            ConnectFourDifficulty.Hard => GetBestMove(5),
            _ => GetBestMove(5)
        };

        if (col >= 0) DropDisc(col, 2);
    }

    private int GetRandomMove()
    {
        var available = AvailableColumns();
        return available.Count == 0 ? -1 : available[Random.Shared.Next(available.Count)];
    }

    private int GetMediumMove()
    {
        // Win if possible
        foreach (var col in AvailableColumns())
        {
            DropDisc(col, 2);
            if (GetWinner() == 2) { UndoDisc(col); return col; }
            UndoDisc(col);
        }
        // Block player win
        foreach (var col in AvailableColumns())
        {
            DropDisc(col, 1);
            if (GetWinner() == 1) { UndoDisc(col); return col; }
            UndoDisc(col);
        }
        // Otherwise random
        return GetRandomMove();
    }

    private int GetBestMove(int depth)
    {
        int bestScore = int.MinValue;
        int bestCol = AvailableColumns().FirstOrDefault();

        foreach (var col in AvailableColumns())
        {
            DropDisc(col, 2);
            int score = Minimax(depth - 1, false, int.MinValue, int.MaxValue);
            UndoDisc(col);

            if (score > bestScore)
            {
                bestScore = score;
                bestCol = col;
            }
        }

        return bestCol;
    }

    private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
    {
        int winner = GetWinner();
        if (winner == 2) return 1000 + depth;
        if (winner == 1) return -1000 - depth;

        var cols = AvailableColumns();
        if (depth == 0 || cols.Count == 0) return ScoreBoard();

        if (isMaximizing)
        {
            int best = int.MinValue;
            foreach (var col in cols)
            {
                DropDisc(col, 2);
                best = Math.Max(best, Minimax(depth - 1, false, alpha, beta));
                UndoDisc(col);
                alpha = Math.Max(alpha, best);
                if (beta <= alpha) break;
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            foreach (var col in cols)
            {
                DropDisc(col, 1);
                best = Math.Min(best, Minimax(depth - 1, true, alpha, beta));
                UndoDisc(col);
                beta = Math.Min(beta, best);
                if (beta <= alpha) break;
            }
            return best;
        }
    }

    private int ScoreBoard()
    {
        int score = 0;
        // Prefer center column
        int centerCol = Cols / 2;
        for (int r = 0; r < Rows; r++)
            if (_board[r, centerCol] == 2) score += 3;

        score += ScoreDirection(0, 1);  // horizontal
        score += ScoreDirection(1, 0);  // vertical
        score += ScoreDirection(1, 1);  // diagonal
        score += ScoreDirection(1, -1); // anti-diagonal
        return score;
    }

    private int ScoreDirection(int dr, int dc)
    {
        int score = 0;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (r + dr * 3 >= Rows || r + dr * 3 < 0) continue;
                if (c + dc * 3 >= Cols || c + dc * 3 < 0) continue;

                int ai = 0, human = 0;
                for (int i = 0; i < 4; i++)
                {
                    int cell = _board[r + dr * i, c + dc * i];
                    if (cell == 2) ai++;
                    else if (cell == 1) human++;
                }

                if (human == 0) score += ai switch { 4 => 100, 3 => 5, 2 => 2, _ => 0 };
                if (ai == 0) score -= human switch { 4 => 100, 3 => 5, 2 => 2, _ => 0 };
            }
        }
        return score;
    }

    // ── Game State ───────────────────────────────────────────

    private bool CheckGameOver()
    {
        int winner = GetWinner();
        if (winner > 0)
        {
            _winningCells = GetWinningCells();
            _gameOver = true;

            if (_gameMode == GameMode.SinglePlayer)
            {
                if (winner == 1)
                {
                    Stats.Wins++;
                    Stats.CurrentStreak++;
                    Stats.LongestWinStreak = Math.Max(Stats.LongestWinStreak, Stats.CurrentStreak);
                }
                else
                {
                    Stats.Losses++;
                    Stats.CurrentStreak = 0;
                }
            }

            Stats.GamesPlayed++;
            _ = SaveStats();
            return true;
        }

        if (AvailableColumns().Count == 0)
        {
            _gameOver = true;
            Stats.Draws++;
            Stats.GamesPlayed++;
            Stats.CurrentStreak = 0;
            _ = SaveStats();
            return true;
        }

        return false;
    }

    private int GetWinner()
    {
        foreach (var (dr, dc) in new[] { (0, 1), (1, 0), (1, 1), (1, -1) })
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int p = _board[r, c];
                    if (p == 0) continue;
                    if (r + dr * 3 >= Rows || r + dr * 3 < 0) continue;
                    if (c + dc * 3 >= Cols || c + dc * 3 < 0) continue;

                    if (Enumerable.Range(0, 4).All(i => _board[r + dr * i, c + dc * i] == p))
                        return p;
                }
            }
        }
        return 0;
    }

    private HashSet<(int, int)> GetWinningCells()
    {
        foreach (var (dr, dc) in new[] { (0, 1), (1, 0), (1, 1), (1, -1) })
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    int p = _board[r, c];
                    if (p == 0) continue;
                    if (r + dr * 3 >= Rows || r + dr * 3 < 0) continue;
                    if (c + dc * 3 >= Cols || c + dc * 3 < 0) continue;

                    if (Enumerable.Range(0, 4).All(i => _board[r + dr * i, c + dc * i] == p))
                        return Enumerable.Range(0, 4)
                            .Select(i => (r + dr * i, c + dc * i))
                            .ToHashSet();
                }
            }
        }
        return [];
    }

    private List<int> AvailableColumns() =>
        Enumerable.Range(0, Cols).Where(c => _board[0, c] == 0).ToList();

    private void UndoDisc(int col)
    {
        for (int r = 0; r < Rows; r++)
        {
            if (_board[r, col] != 0)
            {
                _board[r, col] = 0;
                return;
            }
        }
    }

    // ── Stats ────────────────────────────────────────────────

    public async Task ResetStats()
    {
        Stats = new ConnectFourStats();
        await SaveStats();
        StateHasChanged();
    }

    private async Task SaveStats()
    {
        await JS.InvokeVoidAsync("localStorage.setItem", "cf_stats",
            System.Text.Json.JsonSerializer.Serialize(Stats));
    }

    private async Task LoadStats()
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", "cf_stats");
            if (!string.IsNullOrEmpty(json))
                Stats = System.Text.Json.JsonSerializer.Deserialize<ConnectFourStats>(json) ?? new();
        }
        catch { Stats = new ConnectFourStats(); }
    }
}