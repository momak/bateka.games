namespace bateka.games.Services.Results;

/// <summary>Why a service call didn't do what was asked. The message is shown to the user as-is.</summary>
public sealed record ServiceError(ServiceStatus Status, string Message)
{
    public static ServiceError NotFound(string message) => new(ServiceStatus.NotFound, message);
    public static ServiceError Conflict(string message) => new(ServiceStatus.Conflict, message);
    public static ServiceError Invalid(string message) => new(ServiceStatus.Invalid, message);
}
