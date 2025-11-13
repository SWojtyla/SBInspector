using System.Text.Json;
using SBInspector.Shared.Core.Domain;
using MudBlazor;

namespace SBInspector.Shared.Application.Services;

public class ThemeService
{
    private readonly string _themeFilePath;
    private ThemePreset _currentPreset = ThemePreset.Custom;
    private ThemeConfiguration _customTheme;
    
    public event EventHandler? ThemeChanged;

    public ThemeService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDirectory = Path.Combine(appDataPath, "SBInspector");
        
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        
        _themeFilePath = Path.Combine(configDirectory, "theme-config.json");
        _customTheme = LoadThemeConfiguration();
        LoadCurrentPreset();
    }

    public ThemePreset CurrentPreset
    {
        get => _currentPreset;
        private set
        {
            if (_currentPreset != value)
            {
                _currentPreset = value;
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public MudTheme GetCurrentTheme()
    {
        return CurrentPreset switch
        {
            ThemePreset.Light => GetLightTheme(),
            ThemePreset.Dark => GetDarkTheme(),
            ThemePreset.Custom => GetCustomTheme(),
            _ => GetCustomTheme()
        };
    }

    public bool IsDarkMode()
    {
        return CurrentPreset switch
        {
            ThemePreset.Dark => true,
            ThemePreset.Custom => _customTheme.IsDarkMode,
            _ => false
        };
    }

    public ThemeConfiguration GetCustomThemeConfiguration()
    {
        return _customTheme;
    }

    public async Task SetPresetAsync(ThemePreset preset)
    {
        CurrentPreset = preset;
        await SaveCurrentPresetAsync();
    }

    public async Task SaveCustomThemeAsync(ThemeConfiguration config)
    {
        _customTheme = config;
        await SaveThemeConfigurationAsync();
        if (CurrentPreset == ThemePreset.Custom)
        {
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private MudTheme GetLightTheme()
    {
        return new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Blue.Default,
                Secondary = Colors.Gray.Default,
                Background = "#f5f5f5",  // Softer gray instead of very light gray
                Surface = "#fafafa",      // Slightly darker white for less eye strain
                AppbarBackground = Colors.Blue.Default,
                AppbarText = Colors.Shades.White,
                DrawerBackground = "#e8e8e8",  // Softer gray for drawer
                DrawerText = "#212121",        // Darker text for better contrast
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "4px"
            }
        };
    }

    private MudTheme GetDarkTheme()
    {
        return new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Blue.Lighten1,
                Secondary = Colors.Gray.Default,
                Background = "#1e1e1e",
                Surface = "#2d2d30",
                AppbarBackground = "#2d2d30",
                AppbarText = Colors.Shades.White,
                DrawerBackground = "#252526",
                DrawerText = Colors.Shades.White,
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "4px"
            }
        };
    }

    private MudTheme GetCustomTheme()
    {
        if (_customTheme.IsDarkMode)
        {
            return new MudTheme()
            {
                PaletteDark = new PaletteDark()
                {
                    Primary = _customTheme.Primary,
                    Secondary = _customTheme.Secondary,
                    Background = _customTheme.Background,
                    Surface = _customTheme.Surface,
                    AppbarBackground = _customTheme.AppbarBackground,
                    AppbarText = _customTheme.AppbarText,
                    DrawerBackground = _customTheme.DrawerBackground,
                    DrawerText = _customTheme.DrawerText,
                },
                LayoutProperties = new LayoutProperties()
                {
                    DefaultBorderRadius = _customTheme.DefaultBorderRadius
                }
            };
        }
        else
        {
            return new MudTheme()
            {
                PaletteLight = new PaletteLight()
                {
                    Primary = _customTheme.Primary,
                    Secondary = _customTheme.Secondary,
                    Background = _customTheme.Background,
                    Surface = _customTheme.Surface,
                    AppbarBackground = _customTheme.AppbarBackground,
                    AppbarText = _customTheme.AppbarText,
                    DrawerBackground = _customTheme.DrawerBackground,
                    DrawerText = _customTheme.DrawerText,
                },
                LayoutProperties = new LayoutProperties()
                {
                    DefaultBorderRadius = _customTheme.DefaultBorderRadius
                }
            };
        }
    }

    private ThemeConfiguration LoadThemeConfiguration()
    {
        try
        {
            if (File.Exists(_themeFilePath))
            {
                var json = File.ReadAllText(_themeFilePath);
                return JsonSerializer.Deserialize<ThemeConfiguration>(json) ?? GetDefaultThemeConfiguration();
            }
        }
        catch
        {
            // If there's any error reading the config, return default
        }
        
        return GetDefaultThemeConfiguration();
    }

    private void LoadCurrentPreset()
    {
        var presetFilePath = Path.Combine(Path.GetDirectoryName(_themeFilePath)!, "theme-preset.txt");
        try
        {
            if (File.Exists(presetFilePath))
            {
                var presetText = File.ReadAllText(presetFilePath).Trim();
                if (Enum.TryParse<ThemePreset>(presetText, out var preset))
                {
                    _currentPreset = preset;
                }
            }
        }
        catch
        {
            // If there's any error, use default (Custom)
        }
    }

    private async Task SaveThemeConfigurationAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_customTheme, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_themeFilePath, json);
        }
        catch
        {
            // Silently fail if we can't save configuration
        }
    }

    private async Task SaveCurrentPresetAsync()
    {
        var presetFilePath = Path.Combine(Path.GetDirectoryName(_themeFilePath)!, "theme-preset.txt");
        try
        {
            await File.WriteAllTextAsync(presetFilePath, _currentPreset.ToString());
        }
        catch
        {
            // Silently fail if we can't save preset
        }
    }

    private ThemeConfiguration GetDefaultThemeConfiguration()
    {
        return new ThemeConfiguration
        {
            Name = "Custom",
            IsDarkMode = false,
            Primary = "#00836b",
            Secondary = "#6c757d",
            Background = "#f5f5f5",
            Surface = "#ffffff",
            AppbarBackground = "#00836b",
            AppbarText = "#ffffff",
            DrawerBackground = "#f8f9fa",
            DrawerText = "#212529",
            DefaultBorderRadius = "4px"
        };
    }
}
