using Bateka.UI.Theme;
using MudBlazor;

namespace bateka.games.Layout;

/// <summary>
/// games' colours and faces for the shared <see cref="BatekaTheme"/>, matched to wwwroot/css/app.css
/// ("arcade purple &amp; teal"), so dialogs, menus, snackbars, inputs and buttons look like the rest of
/// the app. Orbitron for titles, Nunito for reading. The dark primary is a light purple, so text on
/// primary buttons is near-black there.
/// </summary>
public static class GamesTheme
{
    private static readonly BatekaTypefaces Typefaces = new()
    {
        Sans = ["Nunito", "system-ui", "-apple-system", "Segoe UI", "sans-serif"],
        Display = ["Orbitron", "Nunito", "system-ui", "sans-serif"]
    };

    public static MudTheme Create() => BatekaTheme.Create(Light(), Dark(), Typefaces);

    private static PaletteLight Light() => new()
    {
        Primary = "#6200ea",
        PrimaryContrastText = "#ffffff",
        Secondary = "#00897b",
        Tertiary = "#ff6d00",
        Info = "#1565c0",
        Success = "#2e7d32",
        Warning = "#b45309",
        Error = "#c62828",
        Background = "#f5f4f8",
        Surface = "#ffffff",
        AppbarBackground = "#f5f4f8",
        AppbarText = "#1a1625",
        DrawerBackground = "#f5f4f8",
        TextPrimary = "#1a1625",
        TextSecondary = "#5e5a6b",
        ActionDefault = "#5e5a6b",
        LinesDefault = "#e4e1ec",
        LinesInputs = "#cfcadb",
        TableLines = "#e4e1ec",
        Divider = "#e4e1ec",
        OverlayDark = "rgba(26, 22, 37, 0.45)"
    };

    private static PaletteDark Dark() => new()
    {
        Primary = "#bb86fc",
        PrimaryContrastText = "#160f22",
        Secondary = "#03dac6",
        Tertiary = "#ffab40",
        Info = "#64b5f6",
        Success = "#66bb6a",
        Warning = "#fbbf24",
        Error = "#ef5350",
        Background = "#121016",
        Surface = "#1c1a22",
        AppbarBackground = "#121016",
        AppbarText = "#ece9f3",
        DrawerBackground = "#121016",
        TextPrimary = "#ece9f3",
        TextSecondary = "#a7a1b5",
        ActionDefault = "#a7a1b5",
        LinesDefault = "#2c2836",
        LinesInputs = "#3b3647",
        TableLines = "#2c2836",
        Divider = "#2c2836",
        OverlayDark = "rgba(6, 5, 9, 0.7)"
    };
}
