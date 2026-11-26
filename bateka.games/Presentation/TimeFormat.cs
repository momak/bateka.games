namespace bateka.games.Presentation;

public static class TimeFormat
{
    /// <summary>"01:05" for 65 seconds.</summary>
    public static string MinutesSeconds(double seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);
        return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
    }
}
