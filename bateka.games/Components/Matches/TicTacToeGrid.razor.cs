using bateka.games.Shared.Matches;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

public partial class TicTacToeGrid
{
    private const string WinnerClass = "ttt-cell--winner";

    /// <summary>Nine squares, row by row; null = empty.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<MatchSide?> Cells { get; set; } = [];

    [Parameter] public IReadOnlyList<int> WinningCells { get; set; } = [];

    /// <summary>Your turn and nothing in flight: empty squares can be tapped.</summary>
    [Parameter] public bool CanPlay { get; set; }

    /// <summary>The tapped square (0–8).</summary>
    [Parameter] public EventCallback<int> OnPlay { get; set; }

    private string CellClass(int index) => WinningCells.Contains(index) ? WinnerClass : string.Empty;
}
