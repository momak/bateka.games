using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace bateka.games.Interop;

/// <summary>
/// Copy / share an invite link, through the ES module wwwroot/js/share.js (loaded on first use).
/// A failed JS call answers false — the page then shows the link to copy by hand.
/// </summary>
public sealed class ShareInterop(IJSRuntime js, ILogger<ShareInterop> logger) : IAsyncDisposable
{
    private const string ImportFunction = "import";
    private const string ModulePath = "./js/share.js";
    private const string ShareFunction = "share";
    private const string CanShareFunction = "canShare";
    private const string CopyFunction = "copy";

    private Task<IJSObjectReference>? _module;

    private Task<IJSObjectReference> Module => _module ??= ImportModuleAsync();

    /// <summary>True on phones (and some desktops) with a share sheet.</summary>
    public Task<bool> CanShareAsync() => CallAsync(CanShareFunction);

    public Task<bool> ShareAsync(string title, string text, string url) => CallAsync(ShareFunction, title, text, url);

    public Task<bool> CopyAsync(string text) => CallAsync(CopyFunction, text);

    public async ValueTask DisposeAsync()
    {
        if (_module is not { IsCompletedSuccessfully: true }) return;

        try
        {
            await (await _module).DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // the page is already gone
        }
    }

    private async Task<bool> CallAsync(string function, params object[] args)
    {
        try
        {
            var module = await Module;
            return await module.InvokeAsync<bool>(function, args);
        }
        catch (JSException ex)
        {
            logger.LogWarning(ex, "{Function} failed", function);
            return false;
        }
    }

    private async Task<IJSObjectReference> ImportModuleAsync()
    {
        try
        {
            return await js.InvokeAsync<IJSObjectReference>(ImportFunction, ModulePath);
        }
        catch (JSException)
        {
            _module = null;
            throw;
        }
    }
}
