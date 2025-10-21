namespace SBInspector.Application.Services;

public class ThemeService
{
    private bool _isDarkMode;

    public event Action? OnThemeChanged;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        private set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                OnThemeChanged?.Invoke();
            }
        }
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
