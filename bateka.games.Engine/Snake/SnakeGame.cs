namespace bateka.games.Engine.Snake;

/// <summary>
/// One game of Snake. The page owns the clock: it calls <see cref="Tick"/> every
/// <see cref="IntervalMs"/> and redraws; arrow keys / the d-pad call <see cref="Turn"/>.
/// </summary>
public sealed class SnakeGame(Random random)
{
    private readonly LinkedList<GridPoint> _body = new();
    private readonly List<GridPoint> _food = [];
    private Direction _direction = SnakeRules.StartDirection;
    private Direction _nextDirection = SnakeRules.StartDirection;

    /// <summary>Head first.</summary>
    public IReadOnlyCollection<GridPoint> Body => _body;

    public GridPoint? Head => _body.First?.Value;

    public IReadOnlyList<GridPoint> Food => _food;

    public int Length => _body.Count;

    public int Score { get; private set; }

    public int Level { get; private set; } = 1;

    public bool IsStarted { get; private set; }

    public bool IsOver { get; private set; }

    public bool IsRunning => IsStarted && !IsOver;

    /// <summary>How long the page should wait between steps at the current level.</summary>
    public int IntervalMs => SnakeRules.IntervalForLevel(Level);

    public void Start()
    {
        _body.Clear();
        for (var i = 0; i < SnakeRules.StartLength; i++)
        {
            _body.AddLast(SnakeRules.StartHead with { X = SnakeRules.StartHead.X - i });
        }

        _direction = SnakeRules.StartDirection;
        _nextDirection = SnakeRules.StartDirection;
        Score = 0;
        Level = 1;
        IsOver = false;
        IsStarted = true;

        _food.Clear();
        for (var i = 0; i < SnakeRules.FoodCount; i++)
        {
            SpawnFood();
        }
    }

    /// <summary>
    /// Asks for a new direction from the next step on. Turning straight back into yourself is
    /// ignored (returns false). Compared with the direction actually moved, so two quick turns
    /// within one step can't reverse the snake either.
    /// </summary>
    public bool Turn(Direction direction)
    {
        if (IsOpposite(direction, _direction)) return false;

        _nextDirection = direction;
        return true;
    }

    public SnakeTickOutcome Tick()
    {
        if (!IsRunning) return SnakeTickOutcome.Idle;

        _direction = _nextDirection;
        var next = Step(_body.First!.Value, _direction);

        if (!IsInside(next) || HitsItself(next))
        {
            IsOver = true;
            return SnakeTickOutcome.Crashed;
        }

        _body.AddFirst(next);

        var foodIndex = _food.IndexOf(next);
        if (foodIndex < 0)
        {
            _body.RemoveLast();
            return SnakeTickOutcome.Moved;
        }

        _food.RemoveAt(foodIndex);
        Score += SnakeRules.PointsPerFood * Level;
        Level = SnakeRules.LevelForLength(_body.Count);
        SpawnFood();
        return SnakeTickOutcome.Ate;
    }

    private bool HitsItself(GridPoint next)
    {
        // The tail moves out of the way this step — unless the snake grows (eats) this step.
        var grows = _food.Contains(next);
        var cellsThatStay = grows ? _body.Count : _body.Count - 1;
        return _body.Take(cellsThatStay).Contains(next);
    }

    private void SpawnFood()
    {
        if (_food.Count >= SnakeRules.FoodCount) return;

        for (var attempt = 0; attempt < SnakeRules.FoodSpawnAttempts; attempt++)
        {
            var candidate = new GridPoint(random.Next(SnakeRules.GridSize), random.Next(SnakeRules.GridSize));
            if (IsFree(candidate))
            {
                _food.Add(candidate);
                return;
            }
        }

        // An almost full board: pick from the cells that are actually free (none = no food).
        var free = AllCells().Where(IsFree).ToList();
        if (free.Count > 0)
        {
            _food.Add(free[random.Next(free.Count)]);
        }
    }

    private bool IsFree(GridPoint cell) => !_body.Contains(cell) && !_food.Contains(cell);

    private static IEnumerable<GridPoint> AllCells() =>
        from y in Enumerable.Range(0, SnakeRules.GridSize)
        from x in Enumerable.Range(0, SnakeRules.GridSize)
        select new GridPoint(x, y);

    private static bool IsInside(GridPoint p) =>
        p.X >= 0 && p.X < SnakeRules.GridSize && p.Y >= 0 && p.Y < SnakeRules.GridSize;

    private static GridPoint Step(GridPoint cell, Direction direction) => direction switch
    {
        Direction.Up => cell with { Y = cell.Y - 1 },
        Direction.Down => cell with { Y = cell.Y + 1 },
        Direction.Left => cell with { X = cell.X - 1 },
        Direction.Right => cell with { X = cell.X + 1 },
        _ => cell
    };

    private static bool IsOpposite(Direction a, Direction b) =>
        (a, b) is (Direction.Up, Direction.Down) or (Direction.Down, Direction.Up)
            or (Direction.Left, Direction.Right) or (Direction.Right, Direction.Left);
}
