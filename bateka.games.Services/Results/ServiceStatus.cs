namespace bateka.games.Services.Results;

/// <summary>Why a service call didn't do what was asked. The Api turns it into 404 / 409 / 400.</summary>
public enum ServiceStatus
{
    NotFound,
    Conflict,
    Invalid
}
