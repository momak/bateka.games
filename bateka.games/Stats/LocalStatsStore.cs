using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace bateka.games.Stats;

/// <summary>
/// Reads and writes a game's stats in this browser's localStorage — the one place that does, instead
/// of every page. Temporary until the backend increment, which keeps stats per user on the API.
/// </summary>
public sealed class LocalStatsStore(IJSRuntime js, ILogger<LocalStatsStore> logger)
{
    private const string GetItemFunction = "localStorage.getItem";
    private const string SetItemFunction = "localStorage.setItem";

    /// <summary>The saved stats, or fresh ones when nothing (or something unreadable) is saved.</summary>
    public async Task<T> LoadAsync<T>(string key) where T : class, new()
    {
        try
        {
            var json = await js.InvokeAsync<string?>(GetItemFunction, key);
            return string.IsNullOrEmpty(json) ? new T() : JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch (Exception ex) when (ex is JsonException or JSException)
        {
            logger.LogWarning(ex, "Stats under {Key} couldn't be read; starting from zero", key);
            return new T();
        }
    }

    public async Task SaveAsync<T>(string key, T stats)
    {
        try
        {
            await js.InvokeVoidAsync(SetItemFunction, key, JsonSerializer.Serialize(stats));
        }
        catch (JSException ex)
        {
            // private mode / storage full: the game still works, the stats just aren't kept
            logger.LogWarning(ex, "Stats under {Key} couldn't be saved", key);
        }
    }
}
