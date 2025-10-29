namespace SBInspector.Shared.Application.Services;

/// <summary>
/// Service to manage and share the current connection state across components
/// </summary>
public class ConnectionStateService
{
    private string? _currentConnectionString;

    /// <summary>
    /// Gets or sets the current connection string
    /// </summary>
    public string? CurrentConnectionString
    {
        get => _currentConnectionString;
        set
        {
            if (_currentConnectionString != value)
            {
                _currentConnectionString = value;
                NotifyStateChanged();
            }
        }
    }

    /// <summary>
    /// Event raised when the connection state changes
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Event raised when connections list should be refreshed (e.g., after saving a new connection)
    /// </summary>
    public event Action? OnConnectionsChanged;

    private void NotifyStateChanged() => OnChange?.Invoke();

    /// <summary>
    /// Notify that the connections list has changed and should be refreshed
    /// </summary>
    public void NotifyConnectionsChanged() => OnConnectionsChanged?.Invoke();
}
