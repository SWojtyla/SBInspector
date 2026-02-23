namespace SEBInspector.Maui.Core.Interfaces;

public interface IFileExportService
{
    Task<bool> ExportToFileAsync(string filename, string content, string contentType = "application/json");
}
