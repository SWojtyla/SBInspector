# Privacy Policy for SBInspector

**Last Updated:** November 7, 2025

## Introduction

SBInspector ("the App") is an Azure Service Bus Inspector application available as both a Blazor web application and a .NET MAUI desktop application. This Privacy Policy describes how the App handles your information.

## Developer Information

**Developer:** SWojtyla  
**Contact:** [GitHub Repository](https://github.com/SWojtyla/SBInspector)

## Data Collection and Usage

### What Data Does the App Access?

The App accesses the following information solely for the purpose of inspecting and managing your Azure Service Bus resources:

1. **Azure Service Bus Connection Strings**: Required to connect to your Azure Service Bus namespace
2. **Azure Service Bus Messages**: Message content, metadata, and properties retrieved from your Service Bus queues, topics, and subscriptions
3. **Message Templates**: Custom message templates you create for sending messages
4. **Application Settings**: Your preferences and saved connections

### How is Data Stored?

#### Desktop Application (.NET MAUI)
- **Connection Strings**: Stored encrypted in local files on your device using .NET Data Protection API
  - Location: `Desktop/SBInspector/connections.json` (encrypted)
- **Message Templates**: Stored in `Documents/SBInspector/Templates/templates.json`
- **Exported Messages**: Saved to `Documents/SBInspector/Exports/` when you choose to export

#### Web Application (Blazor Server)
- **Connection Strings**: Stored encrypted in your browser's local storage using .NET Data Protection API
- **Message Templates**: Stored in your browser's local storage
- All data remains in your browser and is not transmitted to any server except your own Azure Service Bus

### Data Security

- **Encryption**: All connection strings are encrypted using Microsoft's Data Protection API before being stored
- **Local Storage Only**: All data is stored locally on your device or in your browser. No information is transmitted to our servers or any third-party services
- **Direct Connection**: The App connects directly to your Azure Service Bus namespace. We do not act as an intermediary or proxy

## Third-Party Services

The App does NOT share any data with third parties. The only external connection is directly to your Azure Service Bus namespace, which you control.

## Data Retention

- **Desktop Application**: Data remains on your local file system until you manually delete it
- **Web Application**: Data remains in your browser's local storage until you clear it or manually delete saved connections

You can delete:
- Individual saved connections through the App's interface
- Message templates through the App's interface
- All data by clearing browser storage (web) or deleting the SBInspector folders (desktop)

## Your Rights and Control

You have complete control over your data:

- **Access**: All data is stored locally and accessible to you at any time
- **Deletion**: You can delete saved connections and templates at any time through the App
- **Portability**: Your data is stored in standard JSON format and can be accessed directly
- **Control**: You can clear all App data by clearing browser storage (web) or deleting local files (desktop)

## Children's Privacy

The App is not intended for use by children under the age of 13. We do not knowingly collect any information from children.

## Azure Service Bus Data

The App acts as a client to your Azure Service Bus namespace:

- Messages and data retrieved from Azure Service Bus are displayed in the App but not stored permanently unless you explicitly export them
- Any modifications to your Service Bus (sending messages, deleting messages, etc.) are performed directly through the Azure Service Bus SDK
- We have no access to your Azure Service Bus data

## Analytics and Tracking

The App does NOT:
- Collect usage analytics
- Track your behavior
- Use cookies (except those required for basic web functionality in the Blazor web version)
- Share any information with advertising networks
- Transmit telemetry data

## File System Access (Desktop Application)

The desktop application requires access to your file system for:
- Storing encrypted connection strings
- Saving message templates
- Exporting messages when you choose to export

All file operations are performed locally on your device.

## Changes to This Privacy Policy

We may update this Privacy Policy from time to time. Any changes will be reflected in the "Last Updated" date at the top of this document. We encourage you to review this Privacy Policy periodically.

## Open Source

SBInspector is open source software. You can review the source code and verify our privacy practices at:  
https://github.com/SWojtyla/SBInspector

## Consent

By using the App, you consent to this Privacy Policy.

## Contact Us

If you have any questions about this Privacy Policy or the App's privacy practices, please contact us through:
- GitHub Issues: https://github.com/SWojtyla/SBInspector/issues

## Legal Compliance

This Privacy Policy is designed to comply with:
- General Data Protection Regulation (GDPR)
- California Consumer Privacy Act (CCPA)
- Microsoft Store Privacy Requirements

## Summary

**In Plain English:**
- The App only stores your Azure Service Bus connection strings and message templates locally on your device
- All sensitive data is encrypted
- We don't collect, track, or share any of your data
- The App connects directly to your Azure Service Bus - we never see your data
- You have complete control and can delete everything at any time
