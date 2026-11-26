using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace bateka.games.Interop;

/// <summary>
/// Snake's drawing, through the ES module wwwroot/js/snake.js (loaded on first use, no globals,
/// no eval). One per app session. A failed JS call is logged and skipped: a missed frame is
/// redrawn on the next step, so it must never take the page down.
/// </summary>
public sealed class SnakeCanvas(IJSRuntime js, ILogger<SnakeCanvas> logger) : IAsyncDisposable
{
    private const string ImportFunction = "import";
    private const string ModulePath = "./js/snake.js";
    private const string DrawFunction = "draw";
    private const string IsTouchDeviceFunction = "isTouchDevice";

    private Task<IJSObjectReference>? _module;

    private Task<IJSObjectReference> Module =>
        _module ??= ImportModuleAsync();

    public async Task DrawAsync(ElementReference canvas, SnakeFrame frame)
    {
        try
        {
            var module = await Module;
            await module.InvokeVoidAsync(DrawFunction, canvas, frame);
        }
        catch (JSException ex)
        {
            logger.LogWarning(ex, "Snake frame couldn't be drawn");
        }
    }

    /// <summary>True on phones and tablets: the page then shows the on-screen arrows.</summary>
    public async Task<bool> IsTouchDeviceAsync()
    {
        try
        {
            var module = await Module;
            return await module.InvokeAsync<bool>(IsTouchDeviceFunction);
        }
        catch (JSException ex)
        {
            // no arrows is the safe fallback: the keyboard still works
            logger.LogWarning(ex, "Touch detection failed; on-screen arrows hidden");
            return false;
        }
    }

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

    private async Task<IJSObjectReference> ImportModuleAsync()
    {
        try
        {
            return await js.InvokeAsync<IJSObjectReference>(ImportFunction, ModulePath);
        }
        catch (JSException)
        {
            // forget the failed import, so the next call tries again
            _module = null;
            throw;
        }
    }
}
