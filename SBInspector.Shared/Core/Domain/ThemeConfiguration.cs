namespace SBInspector.Shared.Core.Domain;

public class ThemeConfiguration
{
    public string Name { get; set; } = "Custom";
    public bool IsDarkMode { get; set; } = false;
    
    // Palette colors
    public string Primary { get; set; } = "#00836b";
    public string Secondary { get; set; } = "#6c757d";
    public string Background { get; set; } = "#f5f5f5";
    public string Surface { get; set; } = "#ffffff";
    public string AppbarBackground { get; set; } = "#00836b";
    public string AppbarText { get; set; } = "#ffffff";
    public string DrawerBackground { get; set; } = "#f8f9fa";
    public string DrawerText { get; set; } = "#212529";
    
    // Layout properties
    public string DefaultBorderRadius { get; set; } = "4px";
}

public enum ThemePreset
{
    Light,
    Dark,
    Custom
}
