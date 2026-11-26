namespace bateka.games.Engine.Snake;

/// <summary>The numbers that make up a Snake game.</summary>
public static class SnakeRules
{
    /// <summary>The board is GridSize × GridSize cells.</summary>
    public const int GridSize = 20;

    /// <summary>How many pieces of food are on the board at once.</summary>
    public const int FoodCount = 3;

    public const int StartLength = 3;

    /// <summary>Where the head starts; the body trails to the left of it.</summary>
    public static readonly GridPoint StartHead = new(12, 10);

    public const Direction StartDirection = Direction.Right;

    /// <summary>Points for one piece of food, times the current level.</summary>
    public const int PointsPerFood = 10;

    /// <summary>One level up for every this many cells grown.</summary>
    public const int GrowthPerLevel = 5;

    /// <summary>Milliseconds between steps at level 1.</summary>
    public const int BaseIntervalMs = 200;

    /// <summary>Each level is this much faster…</summary>
    public const int IntervalStepMs = 20;

    /// <summary>…but never faster than this.</summary>
    public const int MinIntervalMs = 80;

    /// <summary>Random tries for a free cell before looking through every cell.</summary>
    public const int FoodSpawnAttempts = 100;

    public static int LevelForLength(int length) => 1 + (length - StartLength) / GrowthPerLevel;

    public static int IntervalForLevel(int level) =>
        Math.Max(MinIntervalMs, BaseIntervalMs - (level - 1) * IntervalStepMs);
}
