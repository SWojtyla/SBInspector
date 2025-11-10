# Privacy Policy Documentation

## Feature Overview

This feature adds comprehensive privacy policy documentation to the SBInspector repository, specifically prepared for Microsoft Store submission and general compliance with privacy regulations.

## Files Added

### 1. PRIVACY_POLICY.md
**Purpose**: Full legal privacy policy  
**Format**: Markdown  
**Size**: 5.5KB, 129 lines  
**Use Cases**:
- Display on GitHub repository
- Host on website for privacy policy URL
- Legal reference document
- Detailed privacy information for users

**Key Sections**:
- Introduction and developer information
- Data collection and usage details
- Storage methods (web and desktop)
- Data security measures
- Third-party service disclosure (none)
- Data retention policies
- User rights and control
- Children's privacy
- Azure Service Bus data handling
- Analytics and tracking disclosure (none)
- Legal compliance (GDPR, CCPA, Microsoft Store)

### 2. PRIVACY_POLICY_SHORT.md
**Purpose**: Concise privacy policy with Microsoft Store statement  
**Format**: Markdown  
**Size**: 1.7KB, 40 lines  
**Use Cases**:
- Quick reference for users
- App store descriptions
- Microsoft Store submission forms
- Summary documentation

**Special Feature**: Contains ready-to-copy "Privacy Statement for Microsoft Store" at the bottom

### 3. PRIVACY_POLICY.txt
**Purpose**: Plain text version of privacy policy  
**Format**: Plain text  
**Size**: 3.4KB, 83 lines  
**Use Cases**:
- Copy-paste into text-only forms
- Email templates
- Text editor compatibility
- Systems requiring plain text format

### 4. MICROSOFT_STORE_SUBMISSION_GUIDE.md
**Purpose**: Step-by-step submission guide  
**Format**: Markdown  
**Size**: 5.1KB, 118 lines  
**Use Cases**:
- Microsoft Store submission walkthrough
- Understanding permission declarations
- Answering common store questions
- Testing checklist before submission

**Key Sections**:
- Privacy policy URL options (GitHub Pages, raw URL, custom hosting)
- Privacy statement text (ready to copy)
- App permissions declaration guidance
- Data collection declaration answers
- Data sharing declaration answers
- Common Microsoft Store questions with answers
- Testing recommendations

### 5. PRIVACY_README.md
**Purpose**: Quick reference guide  
**Format**: Markdown  
**Size**: 3.6KB, 112 lines  
**Use Cases**:
- Fast lookup of key information
- Overview of all privacy files
- Copy-paste reference for store submission
- Summary of privacy points

**Features**:
- File descriptions with use cases
- Quick copy-paste privacy statement
- Privacy policy URL options
- Key privacy points checklist
- Data declarations summary
- Required permissions list
- Next steps guidance

### README.md Update
Added new "Privacy" section linking to PRIVACY_POLICY.md with a brief description of the app's privacy practices.

## How to Use for Microsoft Store Submission

### Step 1: Privacy Policy URL
Choose one of these options:

1. **GitHub Raw URL** (easiest):
   ```
   https://raw.githubusercontent.com/SWojtyla/SBInspector/main/PRIVACY_POLICY.md
   ```

2. **GitHub Pages** (if enabled):
   ```
   https://swojtyla.github.io/SBInspector/PRIVACY_POLICY.html
   ```

3. **Custom Website**: Copy content from PRIVACY_POLICY.md to your website

### Step 2: Privacy Statement
Copy the statement from the bottom of `PRIVACY_POLICY_SHORT.md`:
```
SBInspector does not collect, use, or share any user data. All data (Azure Service Bus connection strings and message templates) is stored locally on the user's device in encrypted form. The application connects directly to the user's Azure Service Bus namespace and does not transmit any data to third parties or our servers. No analytics, tracking, or advertising services are used.
```

### Step 3: Follow the Guide
Read `MICROSOFT_STORE_SUBMISSION_GUIDE.md` for complete step-by-step instructions.

## Privacy Compliance

### What Data is Handled
1. **Azure Service Bus Connection Strings**
   - Stored locally on device
   - Encrypted using .NET Data Protection API
   - Never transmitted to developer or third parties

2. **Message Templates**
   - User-created templates for sending messages
   - Stored locally on device
   - Never transmitted to developer or third parties

3. **Application Preferences**
   - UI settings and preferences
   - Stored locally

### Where Data is Stored

**Desktop Application (.NET MAUI)**:
- Connection strings: `Desktop/SBInspector/connections.json` (encrypted)
- Templates: `Documents/SBInspector/Templates/templates.json`
- Exports: `Documents/SBInspector/Exports/` (when user exports)

**Web Application (Blazor Server)**:
- Connection strings: Browser local storage (encrypted)
- Templates: Browser local storage

### Security Measures
- Encryption: Microsoft Data Protection API
- Local storage only: No cloud transmission
- Direct Azure connection: No proxy/intermediary
- Open source: Verifiable code

### Compliance Standards
✅ GDPR (General Data Protection Regulation)  
✅ CCPA (California Consumer Privacy Act)  
✅ Microsoft Store Privacy Requirements  
✅ No cookies (except required web functionality)  
✅ No analytics or tracking  
✅ No third-party services  

## Technical Implementation

The privacy policy accurately reflects the app's implementation:

1. **Encryption Service** (`ConnectionStringEncryptionService.cs`):
   - Uses Microsoft.AspNetCore.DataProtection
   - Encrypts all connection strings before storage
   - Purpose: "SBInspector.ConnectionStrings.v1"

2. **Storage Services**:
   - `LocalStorageService.cs`: Browser local storage (web app)
   - `FileStorageService.cs`: File system storage (desktop app)
   - Both use encrypted connection strings

3. **No External Communication**:
   - Only connects to user's Azure Service Bus namespace
   - No telemetry or analytics services
   - No advertising networks
   - No data transmission to developer

## Maintenance

### Updating Privacy Policy
When to update:
- Adding new features that affect data handling
- Changing storage methods
- Adding third-party services (not currently planned)
- Legal requirement changes

Update process:
1. Update `PRIVACY_POLICY.md` with changes
2. Update "Last Updated" date
3. Update `PRIVACY_POLICY_SHORT.md` if needed
4. Regenerate `PRIVACY_POLICY.txt` if needed
5. Update submission guide if Microsoft Store requirements change

### Version Control
Current Version: November 7, 2025  
Location: Repository root  
Format: Markdown (primary), Plain text (secondary)

## Contact and Support

For privacy policy questions:
- GitHub Issues: https://github.com/SWojtyla/SBInspector/issues
- Repository: https://github.com/SWojtyla/SBInspector

## Legal Disclaimer

This privacy policy template is provided as-is and designed to comply with common privacy regulations. The developer is responsible for ensuring compliance with all applicable laws and regulations. Review with legal counsel if specific concerns exist.

## Summary

The privacy policy documentation is comprehensive, accurate, and ready for Microsoft Store submission. It reflects the actual implementation of the app, which:
- Does not collect user data
- Stores data locally with encryption
- Does not use tracking or analytics
- Does not share data with third parties
- Connects directly to user's Azure resources

All necessary files are provided in multiple formats for various use cases, with clear guidance for Microsoft Store submission.
