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

            var uri = new Uri(Nav.Uri);
            var query = uri.Query;

            await JS.InvokeVoidAsync("console.log", "Query:", query);

            if (query.StartsWith("?p=/") || query.StartsWith("?/"))
            {
                var path = query.TrimStart('?').TrimStart('p').TrimStart('=');
                await JS.InvokeVoidAsync("console.log", "Navigating to:", path);
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
