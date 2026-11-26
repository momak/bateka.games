using System.Text.Json;
using System.Text.Json.Serialization;

namespace bateka.games.Shared.Json;

/// <summary>
/// One JSON shape for the API and the client: camelCase properties and enums as camelCase words
/// ("ticTacToe", not 2). Both sides call <see cref="Configure"/>, so they can't disagree. Same as fit.
/// </summary>
public static class GamesJson
{
    public static readonly JsonSerializerOptions Options = Configure(new JsonSerializerOptions(JsonSerializerDefaults.Web));

    public static JsonSerializerOptions Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
}
