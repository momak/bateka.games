namespace bateka.games.Engine.Tests.TestDoubles;

/// <summary>
/// A <see cref="Random"/> that answers from a fixed list (and then keeps answering 0), so food
/// placement and AI coin flips are predictable in tests.
/// </summary>
public sealed class FixedRandom(params int[] values) : Random
{
    private readonly Queue<int> _values = new(values);

    public override int Next() => Take(int.MaxValue);

    public override int Next(int maxValue) => Take(maxValue);

    public override int Next(int minValue, int maxValue) => minValue + Take(maxValue - minValue);

    private int Take(int maxValue)
    {
        var value = _values.Count > 0 ? _values.Dequeue() : 0;
        return maxValue <= 0 ? 0 : Math.Clamp(value, 0, maxValue - 1);
    }
}
