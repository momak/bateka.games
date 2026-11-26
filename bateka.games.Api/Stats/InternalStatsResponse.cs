namespace bateka.games.Api.Stats;

/// <summary>What /internal/stats answers.</summary>
public sealed record InternalStatsResponse(DateTime GeneratedUtc, IReadOnlyList<InternalStat> Stats);
