using MudBlazor;

namespace bateka.games.Presentation;

/// <summary>
/// How a pick-one chip shows its picked: filled with its colour, the others outlined. MudBlazors own
/// "selected" tint on an outlined chip is too faint to see on the dark theme.
/// </summary>
public static class ChipStyle
{
    public static Variant For(bool selected) => selected ? Variant.Filled : Variant.Outlined;
}
