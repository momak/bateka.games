using Microsoft.AspNetCore.Components;

namespace bateka.games.Pages;

public partial class NotFound
{
    [Parameter] public string? Path { get; set; }
    [Inject] private NavigationManager Nav { get; set; } = default!;
}
