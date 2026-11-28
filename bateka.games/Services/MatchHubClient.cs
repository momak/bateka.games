using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Json;
using bateka.games.Shared.Realtime;
using Bateka.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace bateka.games.Services;

/// <summary>
/// The SignalR connection for online matches, one per app session (like Bateka.AI's ChatHubService).
/// Pushes that arrive while the connection is down are lost, so after <see cref="Reconnected"/> the
/// page must call <see cref="WatchAsync"/> again and re-read the match over REST.
/// </summary>
public sealed class MatchHubClient : IAsyncDisposable
{
    private readonly HubConnection _connection;

    public MatchHubClient(MatchesApiClient api, NavigationManager navigation)
    {
        _connection = new HubConnectionBuilder()
            // same base as the REST calls: the BFF origin, or the dev API
            .WithUrl(api.Absolute(MatchHubContract.Path), options =>
            {
                // negotiate goes through the BFF too: an expired login → 401 → login page, like every API call
                options.HttpMessageHandlerFactory = inner =>
                    new SessionExpiredHandler(navigation) { InnerHandler = inner };
            })
            .AddJsonProtocol(options => GamesJson.Configure(options.PayloadSerializerOptions))
            .WithAutomaticReconnect()
            .Build();

        _connection.On<MatchDto>(MatchHubContract.MatchUpdated, match => MatchUpdated?.Invoke(match));
        _connection.Reconnected += _ => Reconnected?.Invoke() ?? Task.CompletedTask;
    }

    /// <summary>A watched match changed (a move, a join, the end).</summary>
    public event Action<MatchDto>? MatchUpdated;

    /// <summary>Back after a drop (phone locked, network change): watch again and re-read.</summary>
    public event Func<Task>? Reconnected;

    public bool IsConnected => _connection.State == HubConnectionState.Connected;

    /// <summary>Starts the connection if it isn't running (a no-op while connecting or reconnecting).</summary>
    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync(ct);
        }
    }

    /// <summary>Send me this match's updates. Read the match after this, not before.</summary>
    public async Task WatchAsync(Guid matchId, CancellationToken ct = default)
    {
        await StartAsync(ct);
        await _connection.InvokeAsync(MatchHubContract.Watch, matchId, ct);
    }

    public async Task UnwatchAsync(Guid matchId, CancellationToken ct = default)
    {
        if (IsConnected)
        {
            await _connection.InvokeAsync(MatchHubContract.Unwatch, matchId, ct);
        }
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
