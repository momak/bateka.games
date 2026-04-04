using MudBlazor;

namespace bateka.games.Layout;

public partial class MainLayout
{
    private MudThemeProvider? _themeProvider;
    private bool _drawerOpen = true;
    private bool _isDarkMode = false;

    private MudTheme _theme = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#6200EA",
            Secondary = "#00BFA5",
            AppbarBackground = "#6200EA",
            Background = "#F5F5F5",
            Surface = "#FFFFFF",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#BB86FC",
            Secondary = "#03DAC6",
            AppbarBackground = "#1F1B24",
            Background = "#121212",
            Surface = "#1E1E1E",
            DrawerBackground = "#1A1A2E",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["'Nunito'", "sans-serif"]
            },
            H4 = new H4Typography
            {
                FontFamily = ["'Orbitron'", "sans-serif"]
            },
            H5 = new H5Typography
            {
                FontFamily = ["'Orbitron'", "sans-serif"]
            }
        }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _themeProvider!.GetSystemDarkModeAsync();
            await _themeProvider!.WatchSystemDarkModeAsync(OnSystemDarkModeChanged);
            StateHasChanged();
        }
    }

    private async Task OnSystemDarkModeChanged(bool isDarkMode)
    {
        _isDarkMode = isDarkMode;
        StateHasChanged();
        await Task.CompletedTask;
    }

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;
    private void ToggleDarkMode() => _isDarkMode = !_isDarkMode;
}
