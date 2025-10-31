using Microsoft.AspNetCore.DataProtection;

namespace SBInspector.Shared.Application.Services;

/// <summary>
/// Service for encrypting and decrypting connection strings using Data Protection API
/// </summary>
public class ConnectionStringEncryptionService
{
    private readonly IDataProtector _protector;
    private const string Purpose = "SBInspector.ConnectionStrings.v1";

    public ConnectionStringEncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(Purpose);
    }

    /// <summary>
    /// Encrypts a connection string
    /// </summary>
    public string Encrypt(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return string.Empty;
        }

        return _protector.Protect(connectionString);
    }

    /// <summary>
    /// Decrypts a connection string
    /// </summary>
    public string Decrypt(string encryptedConnectionString)
    {
        if (string.IsNullOrWhiteSpace(encryptedConnectionString))
        {
            return string.Empty;
        }

        try
        {
            return _protector.Unprotect(encryptedConnectionString);
        }
        catch
        {
            // If decryption fails (e.g., key changed), return empty string
            return string.Empty;
        }
    }

    /// <summary>
    /// Checks if a string appears to be encrypted (basic heuristic)
    /// </summary>
    public bool IsEncrypted(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        // Simple heuristic: Service Bus connection strings typically start with "Endpoint="
        // If it doesn't start with that, it's likely encrypted
        // This is a lightweight check compared to attempting decryption
        return !value.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase);
    }
}
