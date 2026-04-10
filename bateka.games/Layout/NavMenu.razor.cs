using Microsoft.AspNetCore.Components;

namespace bateka.games.Layout;

public partial class NavMenu
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
}
