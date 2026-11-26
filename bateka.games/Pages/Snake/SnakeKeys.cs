using bateka.games.Engine.Snake;

namespace bateka.games.Pages.Snake;

/// <summary>Keyboard controls: arrows or WASD to steer, space to pause.</summary>
public static class SnakeKeys
{
    public const string Pause = " ";

    private const string ArrowUp = "ArrowUp";
    private const string ArrowDown = "ArrowDown";
    private const string ArrowLeft = "ArrowLeft";
    private const string ArrowRight = "ArrowRight";

    public static Direction? ToDirection(string key) => key switch
    {
        ArrowUp or "w" or "W" => Direction.Up,
        ArrowDown or "s" or "S" => Direction.Down,
        ArrowLeft or "a" or "A" => Direction.Left,
        ArrowRight or "d" or "D" => Direction.Right,
        _ => null
    };
}
