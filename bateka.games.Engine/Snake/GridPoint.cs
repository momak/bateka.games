namespace bateka.games.Engine.Snake;

/// <summary>One cell on the Snake board: X is the column, Y the row, (0, 0) top left.</summary>
public readonly record struct GridPoint(int X, int Y);
