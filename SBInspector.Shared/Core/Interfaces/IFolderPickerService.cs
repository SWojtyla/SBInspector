namespace SBInspector.Shared.Core.Interfaces;

/// <summary>
/// Service for selecting folders from the file system
/// </summary>
public interface IFolderPickerService
{
    /// <summary>
    /// Opens a folder picker dialog and returns the selected folder path
    /// </summary>
    /// <returns>The selected folder path, or null if cancelled</returns>
    Task<string?> PickFolderAsync();
}
