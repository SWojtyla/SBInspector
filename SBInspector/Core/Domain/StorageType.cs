namespace SBInspector.Core.Domain;

public enum StorageType
{
    /// <summary>
    /// Use browser localStorage (default for web apps)
    /// </summary>
    LocalStorage,
    
    /// <summary>
    /// Use file system storage on Desktop (recommended for Tauri desktop apps)
    /// </summary>
    FileSystem
}
