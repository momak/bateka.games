namespace bateka.games.Services.Results;

/// <summary>
/// A value, or the reason there isn't one. <c>return dto;</c> and <c>return ServiceError.Invalid(...)</c>
/// both work. Same as fit.
/// </summary>
public readonly record struct ServiceResult<T>(T? Value, ServiceError? Error)
{
    public static implicit operator ServiceResult<T>(T value) => new(value, null);
    public static implicit operator ServiceResult<T>(ServiceError error) => new(default, error);
}
