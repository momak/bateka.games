namespace bateka.games.Engine.ConnectFour;

/// <summary>One slot: row 0 is the top, column 0 the left.</summary>
public readonly record struct BoardCell(int Row, int Column);
