using bateka.games.Api.Extensions;
using bateka.games.Data.Extensions;
using bateka.games.Repositories.Extensions;
using bateka.games.Services.Extensions;
using Npgsql;
using Serilog;
using Serilog.Events;

const double SlowRequestMs = 5000;

LoggingExtensions.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog: console always; in Development also the VS Output window
    builder.AddGamesLogging();

    // Each part registers itself through its own extension method, so this file stays a list of
    // what the app is made of rather than how any one part is wired (same as fit and BillTracker).
    var dataSettings = builder.Services.AddGamesDataSettings(builder.Configuration);

    builder.Services
        .AddGamesControllers()
        .AddGamesCors(builder.Configuration)
        .AddGamesPersistence(dataSettings.DatabaseConnection)   // bateka.games.Data
        .AddGamesRepositories()                                 // bateka.games.Repositories
        .AddGamesServices()                                     // bateka.games.Services
        .AddGamesSecurity()
        .AddGamesHealthChecks()
        .AddGamesInternalStats()
        .AddGamesRealtime();                                    // SignalR hub + match clock

    var app = builder.Build();

    // Creates the database on first run and applies pending migrations, before taking requests.
    await app.Services.MigrateGamesDatabaseAsync();

    // Name only — the connection string itself carries the password.
    Log.Information("Database : {Database}", new NpgsqlConnectionStringBuilder(dataSettings.DatabaseConnection).Database);

    // Health endpoints and the stats for Bateka.Healthchecks: first, so they answer without the
    // API key / user (not routed by Bateka.BFF, so not public) and aren't request-logged every minute.
    app.UseGamesHealthChecks();
    app.UseGamesInternalStats();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.GetLevel = (ctx, elapsed, ex) =>
        {
            if (ex is not null) return LogEventLevel.Error;
            if (ctx.Response.StatusCode > 499) return LogEventLevel.Error;
            if (elapsed > SlowRequestMs) return LogEventLevel.Warning;
            return LogEventLevel.Information;
        };
    });

    // CORS before the security checks, so a dev preflight (OPTIONS) is answered without them
    app.UseGamesCors();
    app.UseGamesSecurity();
    app.MapControllers();
    app.MapGamesHubs();

    // the URLs are only known once Kestrel has started
    app.Lifetime.ApplicationStarted.Register(() =>
        Log.Information("{Application} started on {Environment} — listening on {Urls}",
            LoggingExtensions.ApplicationName, builder.Environment.EnvironmentName, string.Join(", ", app.Urls)));

    await app.RunAsync();
}
// HostAbortedException is not a failure: `dotnet ef` builds the app to read the model and then
// stops it on purpose — let it through instead of logging a fatal start-up error.
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "{Application} failed to start", LoggingExtensions.ApplicationName);

    // A failed start must not look like a clean exit ("exited with code 0") in docker logs.
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}
