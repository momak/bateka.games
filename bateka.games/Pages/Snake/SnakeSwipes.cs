using bateka.games.Engine.Snake;

namespace bateka.games.Pages.Snake;

/// <summary>The directions snake.js reports for a swipe on the board.</summary>
public static class SnakeSwipes
{
    private const string Up = "up";
    private const string Down = "down";
    private const string Left = "left";
    private const string Right = "right";

    public static Direction? ToDirection(string swipe) => swipe switch
    {
        Up => Direction.Up,
        Down => Direction.Down,
        Left => Direction.Left,
        Right => Direction.Right,
        _ => null
    };
}
