using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace bateka.games.Components;

public partial class StatRow
{
    [Parameter, EditorRequired] public string Label { get; set; } = "";
    [Parameter, EditorRequired] public string Value { get; set; } = "";
    [Parameter] public Color Color { get; set; } = Color.Default;
}