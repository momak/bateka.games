using Microsoft.AspNetCore.Components;

namespace bateka.games.Components;

public partial class GameCard
{
    [Inject] private NavigationManager Nav { get; set; } = default!;

    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter, EditorRequired] public string Description { get; set; } = "";
    [Parameter, EditorRequired] public string Icon { get; set; } = "";
    [Parameter, EditorRequired] public string Href { get; set; } = "";
    [Parameter] public string Color { get; set; } = "#6200EA";
}
