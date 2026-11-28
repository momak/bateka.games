namespace bateka.games.Services;

/// <summary>The signed-in player's name, asked once per app session and then remembered.</summary>
public sealed class CurrentPlayer(PlayersApiClient players)
{
    private Task<string>? _username;

    public Task<string> GetUsernameAsync() => _username ??= LoadAsync();

    private async Task<string> LoadAsync()
    {
        try
        {
            return (await players.GetMeAsync()).Username;
        }
        catch
        {
            // ask again next time instead of remembering the failure
            _username = null;
            throw;
        }
    }
}
