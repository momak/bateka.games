using bateka.games.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace bateka.games.Api.Results;

/// <summary>Turns service results into HTTP answers: 200, or a ProblemDetails with the message as "detail". Same as fit.</summary>
public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result) =>
        result.Error is { } error ? controller.ToProblem(error) : controller.Ok(result.Value);

    private static IActionResult ToProblem(this ControllerBase controller, ServiceError error) =>
        controller.Problem(detail: error.Message, statusCode: error.Status switch
        {
            ServiceStatus.NotFound => StatusCodes.Status404NotFound,
            ServiceStatus.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        });
}
