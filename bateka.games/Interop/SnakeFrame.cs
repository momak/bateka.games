using bateka.games.Engine.Snake;

namespace bateka.games.Interop;

/// <summary>Everything snake.js needs to draw one frame (sent to JS as camelCase JSON).</summary>
public sealed record SnakeFrame(
    int GridSize,
    int CellSize,
    IReadOnlyCollection<GridPoint> Body,
    IReadOnlyList<GridPoint> Food,
    GridPoint? Head,
    bool IsOver);
