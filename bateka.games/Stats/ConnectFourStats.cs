using bateka.games.Engine.ConnectFour;

namespace bateka.games.Stats;

/// <summary>Games against the computer only; two-player games on one device aren't counted.</summary>
public sealed class ConnectFourStats
{
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestWinStreak { get; set; }

    public void Record(ConnectFourResult result)
    {
        switch (result)
        {
            case ConnectFourResult.RedWins:
                Wins++;
                CurrentStreak++;
                LongestWinStreak = Math.Max(LongestWinStreak, CurrentStreak);
                break;
            case ConnectFourResult.YellowWins:
                Losses++;
                CurrentStreak = 0;
                break;
            case ConnectFourResult.Draw:
                Draws++;
                CurrentStreak = 0;
                break;
            default:
                return; // not finished: nothing to count
        }

        GamesPlayed++;
    }
}
