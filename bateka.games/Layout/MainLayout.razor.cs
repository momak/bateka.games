using MudBlazor;

namespace bateka.games.Layout;

/// <summary>games' frame: the shared <c>BatekaShell</c> with games' theme, logo, name and pages.</summary>
public partial class MainLayout
{
    private readonly MudTheme _theme = GamesTheme.Create();
}
