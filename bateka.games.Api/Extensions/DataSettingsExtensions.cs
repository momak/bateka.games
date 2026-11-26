using bateka.games.Api.Configuration;

namespace bateka.games.Api.Extensions;

public static class DataSettingsExtensions
{
    /// <summary>
    /// Reads and validates "ConnectionStrings". Throws at startup when the database connection is
    /// missing, so a broken .env shows up as a clear message instead of a crash on the first request.
    /// </summary>
    public static DataSettings AddGamesDataSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DataSettings>()
            .BindConfiguration(DataSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = configuration.GetSection(DataSettings.SectionName).Get<DataSettings>() ?? new DataSettings();

        if (string.IsNullOrWhiteSpace(settings.DatabaseConnection))
            throw new InvalidOperationException(
                $"{DataSettings.SectionName}:{nameof(DataSettings.DatabaseConnection)} must be configured " +
                $"(environment variable {DataSettings.SectionName}__{nameof(DataSettings.DatabaseConnection)}, or user-secrets locally).");

        return settings;
    }
}
