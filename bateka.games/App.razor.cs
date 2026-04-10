using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace bateka.games;

public partial class App : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private ErrorBoundary? _errorBoundary;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var path = await JS.InvokeAsync<string?>("eval", "window.__spaRedirectPath");
            if (!string.IsNullOrEmpty(path))
            {
                await JS.InvokeVoidAsync("eval", "window.__spaRedirectPath = null");
                NavigationManager.NavigateTo(path, replace: true);
            }
        }
    }

    private void RecoverFromError() => _errorBoundary?.Recover();

    private void ToHome()
    {
        _errorBoundary?.Recover();
        NavigationManager.NavigateTo("");
    }
}
