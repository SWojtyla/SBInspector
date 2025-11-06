namespace SBInspector.Shared.Core.Domain;

public class StorageConfiguration
{
    public StorageType StorageType { get; set; } = StorageType.LocalStorage;
    public string ExportPath { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public string ConnectionsPath { get; set; } = string.Empty;
}
