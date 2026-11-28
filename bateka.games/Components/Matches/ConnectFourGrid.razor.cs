using bateka.games.Engine.ConnectFour;
using bateka.games.Shared.Matches;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Components.Matches;

public partial class ConnectFourGrid
{
    private const string Empty = "cf4-disc--empty";
    private const string Red = "cf4-disc--red";
    private const string Yellow = "cf4-disc--yellow";
    private const string Winner = " cf4-disc--winner";

    /// <summary>42 slots, row by row from the top-left; null = empty.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<MatchSide?> Cells { get; set; } = [];

    [Parameter] public IReadOnlyList<int> WinningCells { get; set; } = [];

    /// <summary>Your turn and nothing in flight.</summary>
    [Parameter] public bool CanPlay { get; set; }

    /// <summary>The column (0–6) of the tapped slot. The server refuses a full column.</summary>
    [Parameter] public EventCallback<int> OnPlay { get; set; }

    private Task PlayColumnAsync(int index) =>
        CanPlay ? OnPlay.InvokeAsync(index % ConnectFourBoard.Columns) : Task.CompletedTask;

    private string DiscClass(int index)
    {
        var colour = Cells[index] switch
        {
            MatchSide.First => Red,
            MatchSide.Second => Yellow,
            _ => Empty
        };

        return WinningCells.Contains(index) ? colour + Winner : colour;
    }
}
