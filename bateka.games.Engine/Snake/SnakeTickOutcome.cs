namespace bateka.games.Engine.Snake;

/// <summary>What one step of the snake did.</summary>
public enum SnakeTickOutcome
{
    /// <summary>Nothing happened: the game isn't running.</summary>
    Idle,

    /// <summary>Moved one cell.</summary>
    Moved,

    /// <summary>Moved onto food: grew by one and scored.</summary>
    Ate,

    /// <summary>Hit a wall or itself: the game is over.</summary>
    Crashed
}
