using SBInspector.Shared.Core.Interfaces;

namespace SEBInspector.Maui.Services;

/// <summary>
/// MAUI implementation of folder picker using platform-specific APIs
/// </summary>
public class MauiFolderPickerService : IFolderPickerService
{
    public async Task<string?> PickFolderAsync()
    {
        try
        {
#if WINDOWS
            // Use Windows.Storage.Pickers for Windows platform
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            
            // Get the window handle for the current window
            if (Application.Current?.Windows.Count > 0)
            {
                var mauiWindow = Application.Current.Windows[0];
                var handler = mauiWindow.Handler as Microsoft.Maui.Handlers.WindowHandler;
                if (handler?.PlatformView is Microsoft.UI.Xaml.Window uiWindow)
                {
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(uiWindow);
                    WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
                }
            }

            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            return folder?.Path;
#else
            // For other platforms, return null (not supported)
            await Task.CompletedTask;
            return null;
#endif
        }
        catch (Exception)
        {
            return null;
        }
    }
}
