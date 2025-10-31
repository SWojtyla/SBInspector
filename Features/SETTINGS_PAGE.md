# Settings Page

## Overview

The Settings page allows users to configure how SBInspector stores data, where exported messages are saved, and provides secure connection string storage using encryption.

## Features

### Storage Location
- **Browser Local Storage (Web)**: Data is stored in the browser's localStorage. Best for web deployments.
- **File System (Desktop)**: Data is stored in files on your computer. Best for desktop applications.

### Export Path Configuration
- Configure where exported messages will be saved
- Default location: `Documents/SBInspector/Exports`
- Can be customized to any path on your system
- Reset button to restore default path

### Template Storage Path Configuration
- Configure where message templates will be saved
- Default location: `Documents/SBInspector/Templates`
- Can be customized to any path on your system
- Reset button to restore default path

### Connection String Security

Connection strings are now stored securely using Microsoft's Data Protection API, which:
- Encrypts connection strings using machine-specific keys
- Protects sensitive data from unauthorized access
- Automatically encrypts new connections when saved
- Provides a migration tool to encrypt existing plain-text connections

#### Encrypting Existing Connections

If you have saved connections from a previous version that were stored in plain text, the Settings page will:
1. Display a warning showing how many unencrypted connections exist
2. Provide an "Encrypt Now" button to automatically encrypt all connections
3. Confirm when encryption is complete

## Usage

### Accessing Settings

Navigate to the Settings page by:
1. Clicking on the "Settings" link in the left navigation menu
2. Or directly accessing `/settings` URL

### Configuring Storage

1. Select your preferred storage type from the dropdown
2. The page will show a description of the selected storage type
3. Click "Save Settings" to apply changes

### Configuring Paths

1. Enter custom paths for exports and templates in the respective text fields
2. Leave fields empty to use default paths
3. Click the "Reset" button next to each field to clear and use defaults
4. Click "Save Settings" to apply changes

### Encrypting Connections

1. If you have unencrypted connections, a warning will be displayed
2. Click the "Encrypt Now" button
3. Wait for the encryption process to complete
4. The page will confirm when all connections are encrypted

## Technical Details

### Encryption

- Uses ASP.NET Core Data Protection API
- Keys are stored in: `%APPDATA%/SBInspector/Keys` (Windows) or `~/.local/share/SBInspector/Keys` (Linux/Mac)
- Each machine has unique encryption keys
- Connection strings are encrypted before being saved to storage
- Decryption happens automatically when loading connections

### Storage Configuration

Settings are stored in: `%APPDATA%/SBInspector/storage-config.json` (Windows) or `~/.local/share/SBInspector/storage-config.json` (Linux/Mac)

The configuration file includes:
- Selected storage type
- Custom export path (if configured)
- Custom template path (if configured)

### File Storage Paths

When using File System storage:
- **Connection data**: `Desktop/SBInspector/connections.json`
- **Templates**: Configured path (default: `Documents/SBInspector/Templates/templates.json`)
- **Exports**: Saved to configured path (default: `Documents/SBInspector/Exports/`)

## Security Considerations

1. **Machine-Specific Keys**: Encryption keys are tied to the machine where they were created. Moving the data to another machine will require re-entering connection strings.

2. **Backup**: If you back up your connection data, ensure you also back up the encryption keys from the Keys directory.

3. **Migration**: When upgrading from a version without encryption, use the "Encrypt Now" feature to secure your existing connections.

## Screenshots

![Settings Page](https://github.com/user-attachments/assets/bafbb726-d20c-46d6-997d-225f2fd3a4f6)

The Settings page showing:
- Storage location configuration
- Export path configuration
- Template storage path configuration
- Connection string security status
- Helpful information panel

## Related Features

- [Connection Management](CONNECTION_TREE_VIEW.md)
- [Message Templates](../README.md#message-templates)
- [Export Functionality](SINGLE_MESSAGE_EXPORT.md)
