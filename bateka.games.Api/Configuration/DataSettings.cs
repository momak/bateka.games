using System.ComponentModel.DataAnnotations;

namespace bateka.games.Api.Configuration;

/// <summary>
/// Connection strings, bound from "ConnectionStrings" — in Docker they arrive as
/// ConnectionStrings__DatabaseConnection. Same name as fit, Bateka.AI and Bateka.Healthchecks.
/// </summary>
public sealed class DataSettings
{
    public const string SectionName = "ConnectionStrings";

    /// <summary>Npgsql connection string for this app's own database (batekaGames) on the shared Postgres.</summary>
    [Required]
    public string DatabaseConnection { get; set; } = string.Empty;
}
