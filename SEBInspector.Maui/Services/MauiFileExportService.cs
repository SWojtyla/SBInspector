using SBInspector.Shared.Core.Interfaces;

namespace SEBInspector.Maui.Services;

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

// Helper class for file saving in MAUI
public static class FileSaver
{
    public static IFileSaver Default { get; } = new DefaultFileSaver();
}

public interface IFileSaver
{
    Task<string?> SaveAsync(string filename, string content);
}

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
