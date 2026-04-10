using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace bateka.games;

public partial class App : ComponentBase
{
    [Inject]
    private NavigationManager Nav { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private ErrorBoundary? _errorBoundary;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("console.log", "Current URL:", Nav.Uri);
            await JS.InvokeVoidAsync("console.log", "Base URI:", Nav.BaseUri);

            var path = await JS.InvokeAsync<string?>("eval", "window.__spaRedirectPath");
            await JS.InvokeVoidAsync("console.log", "Redirect path:", path);

            if (!string.IsNullOrEmpty(path))
            {
                await JS.InvokeVoidAsync("eval", "window.__spaRedirectPath = null");
                Nav.NavigateTo(path, replace: true);
            }
        }
    }

    private void RecoverFromError() => _errorBoundary?.Recover();

    private void ToHome()
    {
        _errorBoundary?.Recover();
        Nav.NavigateTo("");
    }
}
