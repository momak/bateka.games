using bateka.games.Engine.Snake;
using bateka.games.Engine.Tests.TestDoubles;

namespace bateka.games.Engine.Tests.Snake;

public sealed class SnakeGameTests
{
    // Food at (0,0), (1,0), (2,0): far away from the snake's row (y = 10).
    private static FixedRandom FoodInTopLeft(params int[] more) => new([0, 0, 1, 0, 2, 0, .. more]);

    [Fact]
    public void Start_puts_a_three_cell_snake_facing_right_and_three_pieces_of_food()
    {
        var game = new SnakeGame(new Random(1));

        game.Start();

        Assert.Equal([new(12, 10), new(11, 10), new(10, 10)], game.Body.ToArray());
        Assert.Equal(SnakeRules.FoodCount, game.Food.Count);
        Assert.All(game.Food, food => Assert.DoesNotContain(food, game.Body));
        Assert.Equal(1, game.Level);
        Assert.Equal(0, game.Score);
        Assert.True(game.IsRunning);
    }

    [Fact]
    public void Tick_moves_the_head_one_cell_and_keeps_the_length()
    {
        var game = new SnakeGame(FoodInTopLeft());
        game.Start();

        var outcome = game.Tick();

        Assert.Equal(SnakeTickOutcome.Moved, outcome);
        Assert.Equal(new GridPoint(13, 10), game.Head);
        Assert.Equal(3, game.Length);
    }

    [Fact]
    public void Tick_before_start_does_nothing()
    {
        var game = new SnakeGame(FoodInTopLeft());

        Assert.Equal(SnakeTickOutcome.Idle, game.Tick());
    }

    [Fact]
    public void Turning_straight_back_is_ignored()
    {
        var game = new SnakeGame(FoodInTopLeft());
        game.Start();

        Assert.False(game.Turn(Direction.Left));
        game.Tick();

        Assert.Equal(new GridPoint(13, 10), game.Head);
    }

    [Fact]
    public void Two_quick_turns_within_one_step_cannot_reverse_the_snake()
    {
        var game = new SnakeGame(FoodInTopLeft());
        game.Start();

        Assert.True(game.Turn(Direction.Up));
        Assert.False(game.Turn(Direction.Left)); // still compared with "right", the direction moved

        game.Tick();
        Assert.Equal(new GridPoint(12, 9), game.Head);
    }

    [Fact]
    public void Running_into_the_wall_ends_the_game()
    {
        var game = new SnakeGame(FoodInTopLeft());
        game.Start();

        // head at x = 12, the last column is x = 19: seven steps fit, the eighth hits the wall
        for (var i = 0; i < 7; i++)
        {
            Assert.Equal(SnakeTickOutcome.Moved, game.Tick());
        }

        Assert.Equal(SnakeTickOutcome.Crashed, game.Tick());
        Assert.True(game.IsOver);
        Assert.Equal(SnakeTickOutcome.Idle, game.Tick());
    }

    [Fact]
    public void Eating_grows_the_snake_scores_and_replaces_the_food()
    {
        // first food right in front of the head, then two far away, then the replacement
        var game = new SnakeGame(new FixedRandom(13, 10, 0, 0, 1, 0, 5, 5));
        game.Start();

        var outcome = game.Tick();

        Assert.Equal(SnakeTickOutcome.Ate, outcome);
        Assert.Equal(4, game.Length);
        Assert.Equal(SnakeRules.PointsPerFood, game.Score);
        Assert.Equal(SnakeRules.FoodCount, game.Food.Count);
        Assert.Contains(new GridPoint(5, 5), game.Food);
    }

    [Fact]
    public void Moving_into_the_cell_the_tail_leaves_is_not_a_crash()
    {
        // eat once at (13,10) so the snake is 4 long, then go round in a tight square
        var game = new SnakeGame(new FixedRandom(13, 10, 0, 0, 1, 0, 2, 0));
        game.Start();
        game.Tick();                 // (13,10), ate

        game.Turn(Direction.Down);
        game.Tick();                 // (13,11)
        game.Turn(Direction.Left);
        game.Tick();                 // (12,11)
        game.Turn(Direction.Up);
        var outcome = game.Tick();   // (12,10): where the tail was a moment ago

        Assert.Equal(SnakeTickOutcome.Moved, outcome);
        Assert.Equal(new GridPoint(12, 10), game.Head);
    }

    [Fact]
    public void Running_into_its_own_body_ends_the_game()
    {
        // eat twice to be 5 long, then turn into the body
        var game = new SnakeGame(new FixedRandom(13, 10, 14, 10, 0, 0, 1, 0, 2, 0));
        game.Start();
        game.Tick();                 // (13,10), ate
        game.Tick();                 // (14,10), ate

        game.Turn(Direction.Down);
        game.Tick();                 // (14,11)
        game.Turn(Direction.Left);
        game.Tick();                 // (13,11)
        game.Turn(Direction.Up);

        Assert.Equal(SnakeTickOutcome.Crashed, game.Tick()); // (13,10) is body, not the tail
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(13, 3)]
    public void Level_goes_up_every_five_cells_grown(int length, int level)
    {
        Assert.Equal(level, SnakeRules.LevelForLength(length));
    }

    [Theory]
    [InlineData(1, 200)]
    [InlineData(2, 180)]
    [InlineData(7, 80)]
    [InlineData(20, 80)]
    public void Each_level_is_faster_down_to_the_minimum(int level, int intervalMs)
    {
        Assert.Equal(intervalMs, SnakeRules.IntervalForLevel(level));
    }
}
