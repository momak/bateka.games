namespace bateka.games.Data;

/// <summary>Column lengths that aren't user input.</summary>
public static class ColumnLengths
{
    public const int UserId = 256;

    /// <summary>Enums are stored as their name ("TicTacToe"), so a SQL client shows what they mean.</summary>
    public const int EnumName = 20;
}
