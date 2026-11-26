namespace bateka.games.Engine.ConnectFour;

/// <summary>The 6 × 7 grid. Discs fall to the lowest free slot of a column.</summary>
public sealed class ConnectFourBoard
{
    public const int Rows = 6;
    public const int Columns = 7;
    public const int InARow = 4;

    /// <summary>Right, down, down-right, down-left: every line of four is found from its first cell.</summary>
    private static readonly (int Dr, int Dc)[] Directions = [(0, 1), (1, 0), (1, 1), (1, -1)];

    private readonly Disc[,] _slots = new Disc[Rows, Columns];

    public Disc this[int row, int column] => _slots[row, column];

    public bool CanDrop(int column) =>
        column is >= 0 and < Columns && _slots[0, column] == Disc.None;

    public bool IsFull => Enumerable.Range(0, Columns).All(c => !CanDrop(c));

    public List<int> AvailableColumns() => Enumerable.Range(0, Columns).Where(CanDrop).ToList();

    /// <summary>Drops a disc; returns the row it landed in, or -1 when the column is full.</summary>
    public int Drop(int column, Disc disc)
    {
        if (!CanDrop(column)) return -1;

        for (var row = Rows - 1; row >= 0; row--)
        {
            if (_slots[row, column] == Disc.None)
            {
                _slots[row, column] = disc;
                return row;
            }
        }

        return -1;
    }

    /// <summary>Takes the top disc out of a column again (the AI tries moves this way).</summary>
    public void Undo(int column)
    {
        for (var row = 0; row < Rows; row++)
        {
            if (_slots[row, column] != Disc.None)
            {
                _slots[row, column] = Disc.None;
                return;
            }
        }
    }

    public void Clear() => Array.Clear(_slots);

    /// <summary>The four winning slots, or an empty set.</summary>
    public IReadOnlySet<BoardCell> WinningCells()
    {
        foreach (var (row, column, dr, dc) in LineStarts())
        {
            var disc = _slots[row, column];
            if (disc != Disc.None && IsLineOf(disc, row, column, dr, dc))
            {
                return Enumerable.Range(0, InARow)
                    .Select(i => new BoardCell(row + dr * i, column + dc * i))
                    .ToHashSet();
            }
        }

        return new HashSet<BoardCell>();
    }

    public Disc Winner()
    {
        foreach (var (row, column, dr, dc) in LineStarts())
        {
            var disc = _slots[row, column];
            if (disc != Disc.None && IsLineOf(disc, row, column, dr, dc)) return disc;
        }

        return Disc.None;
    }

    /// <summary>Counts of each colour in every window of four (for the AI's position score).</summary>
    public IEnumerable<(int Mine, int Theirs)> Windows(Disc mine)
    {
        foreach (var (row, column, dr, dc) in LineStarts())
        {
            int own = 0, other = 0;
            for (var i = 0; i < InARow; i++)
            {
                var disc = _slots[row + dr * i, column + dc * i];
                if (disc == mine) own++;
                else if (disc != Disc.None) other++;
            }

            yield return (own, other);
        }
    }

    private bool IsLineOf(Disc disc, int row, int column, int dr, int dc)
    {
        for (var i = 1; i < InARow; i++)
        {
            if (_slots[row + dr * i, column + dc * i] != disc) return false;
        }

        return true;
    }

    /// <summary>Every (start cell, direction) whose line of four stays on the board.</summary>
    private static IEnumerable<(int Row, int Column, int Dr, int Dc)> LineStarts()
    {
        foreach (var (dr, dc) in Directions)
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var endRow = row + dr * (InARow - 1);
                    var endColumn = column + dc * (InARow - 1);
                    if (endRow is >= 0 and < Rows && endColumn is >= 0 and < Columns)
                    {
                        yield return (row, column, dr, dc);
                    }
                }
            }
        }
    }
}
