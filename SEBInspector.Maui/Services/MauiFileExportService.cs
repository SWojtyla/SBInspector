using SBInspector.Shared.Core.Interfaces;

namespace SEBInspector.Maui.Services;

/// <summary>
/// MAUI implementation of file export service for saving files to the local file system
/// </summary>
public class MauiFileExportService : IFileExportService
{
    public async Task<bool> ExportToFileAsync(string filename, string content, string contentType = "application/json")
    {
        try
        {
            // Use the file saver to save to Downloads/Documents folder
            var filePath = await FileSaver.Default.SaveAsync(filename, content);
            
            if (filePath != null)
            {
                // Show alert with file location on main thread
                var page = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0].Page : null;
                if (page is not null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        page.DisplayAlert(
                            "Export Successful",
                            $"File saved to:\n{filePath}",
                            "OK"));
                }
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            // Show error on main thread
            var page = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0].Page : null;
            if (page is not null)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    page.DisplayAlert(
                        "Export Failed",
                        $"Error saving file: {ex.Message}",
                        "OK"));
            }
            return false;
        }
    }
}

/// <summary>
/// Helper class for file saving in MAUI
/// </summary>
public static class FileSaver
{
    /// <summary>
    /// Gets the default file saver implementation
    /// </summary>
    public static IFileSaver Default { get; } = new DefaultFileSaver();
}

/// <summary>
/// Interface for file saving operations
/// </summary>
public interface IFileSaver
{
    /// <summary>
    /// Saves content to a file with the specified filename
    /// </summary>
    /// <param name="filename">The name of the file to save</param>
    /// <param name="content">The content to write to the file</param>
    /// <returns>The full path of the saved file, or null if the operation failed</returns>
    Task<string?> SaveAsync(string filename, string content);
}

/// <summary>
/// Default implementation of file saver that saves files to the Downloads or Documents folder
/// </summary>
public class DefaultFileSaver : IFileSaver
{
    public async Task<string?> SaveAsync(string filename, string content)
    {
        try
        {
            // Get the Downloads folder or Documents folder
            var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            // For desktop platforms, try to get Downloads folder
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var downloads = Path.Combine(userProfile, "Downloads");
                if (Directory.Exists(downloads))
                {
                    downloadsPath = downloads;
                }
            }

            // Create full file path
            var filePath = Path.Combine(downloadsPath, filename);
            
            // If file exists, add a number to make it unique
            int counter = 1;
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            
            while (File.Exists(filePath))
            {
                var newFilename = $"{fileNameWithoutExt}_{counter}{extension}";
                filePath = Path.Combine(downloadsPath, newFilename);
                counter++;
            }
            
            // Write the file
            await File.WriteAllTextAsync(filePath, content);
            
            return filePath;
        }
        catch
        {
            return null;
        }
    }
}
