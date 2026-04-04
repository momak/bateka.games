using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace bateka.games;

public partial class App
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private ErrorBoundary? _errorBoundary;

    private void RecoverFromError() => _errorBoundary?.Recover();

    private void ToHome()
    {
        _errorBoundary?.Recover();
        NavigationManager.NavigateTo("Lists");
    }
}
