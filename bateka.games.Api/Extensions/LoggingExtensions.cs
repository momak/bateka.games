using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace bateka.games.Api.Extensions;

/// <summary>
/// Serilog, same setup as Bateka.AI: levels from the "Serilog" section, console output always.
/// In Development also the Visual Studio Output window (Debug) and a daily file in logs/ — so the
/// log is there even when the API's console window is hidden behind VS.
/// </summary>
public static class LoggingExtensions
{
    public const string ApplicationName = "bateka.games.Api";

    private const string ConsoleTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}      {Message:lj}{NewLine}{Exception}";

    private const string FileTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} | {Message:lj} {Properties:j}{NewLine}{Exception}";

    private const string LogFilePath = "logs/games-api-.log";
    private const int RetainedLogFiles = 7;

    /// <summary>Logs start-up problems before the host (and its configuration) exists.</summary>
    public static void CreateBootstrapLogger() =>
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.Debug()
            .CreateBootstrapLogger();

    public static WebApplicationBuilder AddGamesLogging(this WebApplicationBuilder builder)
    {
        var isDevelopment = builder.Environment.IsDevelopment();

        builder.Host.UseSerilog((ctx, services, config) =>
        {
            config
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", ApplicationName)
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: ConsoleTemplate);

            if (isDevelopment)
            {
                config
                    .WriteTo.Debug(outputTemplate: ConsoleTemplate)
                    .WriteTo.File(LogFilePath, rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: RetainedLogFiles, outputTemplate: FileTemplate);
            }
        });

        return builder;
    }
}
