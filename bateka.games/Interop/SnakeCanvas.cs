using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace bateka.games.Interop;

/// <summary>
/// Snake's drawing and touch steering, through the ES module wwwroot/js/snake.js (loaded on first
/// use, no globals, no eval). One per app session. A failed JS call is logged and skipped: a missed
/// frame is redrawn on the next step, so it must never take the page down.
/// </summary>
public sealed class SnakeCanvas(IJSRuntime js, ILogger<SnakeCanvas> logger) : IAsyncDisposable
{
    /// <summary>The [JSInvokable] method snake.js calls with "up" / "down" / "left" / "right".</summary>
    public const string SwipeCallback = "OnSwipe";

    private const string ImportFunction = "import";
    private const string ModulePath = "./js/snake.js";
    private const string DrawFunction = "draw";
    private const string IsTouchDeviceFunction = "isTouchDevice";
    private const string AttachSwipeFunction = "attachSwipe";
    private const string DetachSwipeFunction = "detachSwipe";

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

    /// <summary>True on phones and tablets: the page then shows the arrows and listens for swipes.</summary>
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

    /// <summary>Swipes on <paramref name="board"/> call <see cref="SwipeCallback"/> on <paramref name="receiver"/>.</summary>
    public async Task AttachSwipeAsync<T>(ElementReference board, DotNetObjectReference<T> receiver) where T : class
    {
        try
        {
            var module = await Module;
            await module.InvokeVoidAsync(AttachSwipeFunction, board, receiver);
        }
        catch (JSException ex)
        {
            // the arrow buttons still steer
            logger.LogWarning(ex, "Swipe steering couldn't be attached");
        }
    }

    public async Task DetachSwipeAsync(ElementReference board)
    {
        if (_module is not { IsCompletedSuccessfully: true }) return;

        try
        {
            await (await _module).InvokeVoidAsync(DetachSwipeFunction, board);
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException)
        {
            // the page is already gone
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
