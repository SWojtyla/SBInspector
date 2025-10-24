namespace SBInspector.Shared.Core.Interfaces;

public interface IFileExportService
{
    Task<bool> ExportToFileAsync(string filename, string content, string contentType = "application/json");
}
