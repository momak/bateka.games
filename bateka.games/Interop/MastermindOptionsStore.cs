using System.Text.Json;
using bateka.games.Presentation;
using bateka.games.Shared.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace bateka.games.Interop;

/// <summary>
/// Remembers the Mastermind options on this device (localStorage). Light by design: losing them only
/// means the classic board next time, so every failure falls back to the defaults.
/// </summary>
public sealed class MastermindOptionsStore(IJSRuntime js, ILogger<MastermindOptionsStore> logger)
{
    private const string Key = "bateka-games-mastermind-options";
    private const string GetItem = "localStorage.getItem";
    private const string SetItem = "localStorage.setItem";

    public async Task<MastermindOptions> LoadAsync()
    {
        try
        {
            var json = await js.InvokeAsync<string?>(GetItem, Key);
            var options = json is null ? null : JsonSerializer.Deserialize<MastermindOptions>(json, GamesJson.Options);
            return options is { IsPlayable: true } ? options : new MastermindOptions();
        }
        catch (Exception ex) when (ex is JSException or JsonException)
        {
            logger.LogWarning(ex, "Saved Mastermind options unreadable; using the classic board");
            return new MastermindOptions();
        }
    }

    public async Task SaveAsync(MastermindOptions options)
    {
        try
        {
            await js.InvokeVoidAsync(SetItem, Key, JsonSerializer.Serialize(options, GamesJson.Options));
        }
        catch (JSException ex)
        {
            // private mode / storage full: they just won't be remembered
            logger.LogWarning(ex, "Couldn't save the Mastermind options");
        }
    }
}
