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
            //await JS.InvokeVoidAsync("console.log", "Current URL:", Nav.Uri);
            //await JS.InvokeVoidAsync("console.log", "Base URI:", Nav.BaseUri);

            var uri = new Uri(Nav.Uri);
            var query = uri.Query;

            //await JS.InvokeVoidAsync("console.log", "Query:", query);
            var path = ExtractQueryPath(query);
            if (!string.IsNullOrEmpty(path))
            {
                //await JS.InvokeVoidAsync("console.log", "Navigating to:", path);
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

    private static string? ExtractQueryPath(string query)
    {
        if (string.IsNullOrEmpty(query))
            return null;

        var span = query.AsSpan();

        // Skip leading '?'
        if (span[0] == '?')
            span = span[1..];

        if (span.IsEmpty)
            return null;

        // Skip 'p' and optional '='
        if (span[0] == 'p')
        {
            span = (span.Length > 1 && span[1] == '=')
                ? span[2..]
                : span[1..];
        }

        // Find terminator (& or ?)
        var endIndex = span.IndexOfAny('&', '?');
        if (endIndex >= 0)
            span = span[..endIndex];

        // Remove leading slashes and validate
        span = span.TrimStart('/');

        return span.IsEmpty ? null : span.ToString();
    }
}
