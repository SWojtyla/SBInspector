namespace SBInspector.Application.Services;

public class ThemeService
{
    private bool _isDarkMode;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        private set => _isDarkMode = value;
    }

    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
    }

    public void SetDarkMode(bool isDarkMode)
    {
        IsDarkMode = isDarkMode;
    }

    public string GetThemeClass()
    {
        return IsDarkMode ? "dark-theme" : "light-theme";
    }
}
