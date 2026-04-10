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
            var path = ExtractQueryPath(query);
            if (!string.IsNullOrEmpty(path))
            {
             //   if (query.StartsWith("?p=/") || query.StartsWith("?/"))
            //{
                //var path = query.TrimStart('?').TrimStart('p').TrimStart('=').TrimStart('/');
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

    private static string? ExtractQueryPath(string query)
    {
        if (string.IsNullOrEmpty(query) || query.Length < 2)
            return null;

        // Remove leading ?
        var normalized = query.StartsWith("?") ? query[1..] : query;

        // Best-effort extraction of p parameter
        if (normalized.StartsWith("p="))
        {
            normalized = normalized[2..];
        }
        else if (normalized.StartsWith("p") && normalized.Length > 1 && normalized[1] != '=')
        {
            // Handle ?pXXX format (malformed, but best effort)
            normalized = normalized[1..];
        }
        else if (normalized.StartsWith("="))
        {
            // Handle ?=XXX format
            normalized = normalized[1..];
        }
        // else: doesn't start with known pattern, use as-is for best effort

        // Extract the value (stop at & or next ?)
        var endIndex = normalized.IndexOfAny(['&', '?']);
        if (endIndex > 0)
        {
            normalized = normalized[..endIndex];
        }

        // Remove leading slashes and validate
        normalized = normalized.TrimStart('/');
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
