namespace bateka.games.Api.Stats;

/// <summary>
/// One number bateka.games reports about itself. Bateka.Healthchecks shows it as given — label, value,
/// unit, tone — so it needs no knowledge of this app. Its copy of the shape is AppStatResponse.
/// </summary>
public sealed record InternalStat(
    string Key,
    string Label,
    double? Value = null,
    string? Unit = null,
    string? Text = null,
    StatTone Tone = StatTone.Neutral,
    string? Hint = null);
