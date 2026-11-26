using bateka.games.Api.Filters;
using bateka.games.Shared.Json;
using bateka.games.Shared.Validation;
using FluentValidation;

namespace bateka.games.Api.Extensions;

public static class ApiExtensions
{
    /// <summary>
    /// Controllers with the shared JSON shape (<see cref="GamesJson"/>) and the global FluentValidation
    /// filter — every request body is checked by its validator from bateka.games.Shared before the
    /// action runs, so no controller calls ValidateAsync itself.
    /// </summary>
    public static IServiceCollection AddGamesControllers(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddValidatorsFromAssemblyContaining<RecordGameResultRequestValidator>();

        services.AddControllers(options => options.Filters.Add<ValidationFilter>())
            .AddJsonOptions(options => GamesJson.Configure(options.JsonSerializerOptions));

        return services;
    }
}
