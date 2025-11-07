# Microsoft Store Privacy Policy Submission Guide

This document provides guidance on using the privacy policy files for Microsoft Store submission.

## Files Included

1. **PRIVACY_POLICY.md** - Comprehensive privacy policy in Markdown format
   - Use for: Website, GitHub repository, detailed documentation
   - Full legal privacy policy with all details

2. **PRIVACY_POLICY_SHORT.md** - Concise version with Microsoft Store statement
   - Use for: Quick reference, app store descriptions
   - Contains a specific "Privacy Statement for Microsoft Store" section at the bottom

3. **PRIVACY_POLICY.txt** - Plain text version
   - Use for: Copying directly into forms or text editors
   - Same content as the full policy but in plain text format

## How to Use for Microsoft Store Submission

### Step 1: Privacy Policy URL
Microsoft Store requires a privacy policy URL. You have two options:

**Option A: Host on GitHub Pages**
1. Enable GitHub Pages in your repository settings
2. Use the URL: `https://swojtyla.github.io/SBInspector/PRIVACY_POLICY.html`
3. You may need to create an HTML version or let GitHub Pages convert the Markdown

**Option B: Use GitHub Raw URL**
Use: `https://raw.githubusercontent.com/SWojtyla/SBInspector/main/PRIVACY_POLICY.md`

**Option C: Host on Your Own Website**
Copy the content from `PRIVACY_POLICY.md` to your website

### Step 2: Microsoft Store Privacy Statement
When submitting to Microsoft Store, you'll be asked to provide a privacy statement. Use the text from the bottom of `PRIVACY_POLICY_SHORT.md`:

```
SBInspector does not collect, use, or share any user data. All data (Azure Service Bus connection strings and message templates) is stored locally on the user's device in encrypted form. The application connects directly to the user's Azure Service Bus namespace and does not transmit any data to third parties or our servers. No analytics, tracking, or advertising services are used.
```

### Step 3: App Permissions Declaration
When declaring app permissions in Microsoft Store, mention:

**Desktop App Permissions:**
- **File System Access**: Required to save encrypted connection strings and message templates locally
- **Network Access**: Required to connect to Azure Service Bus namespaces

**Web App Permissions:**
- **Local Storage**: Required to save encrypted connection strings and message templates in browser
- **Network Access**: Required to connect to Azure Service Bus namespaces

### Step 4: Data Collection Declaration
In Microsoft Store submission, when asked about data collection:

- **Personal Information**: No
- **User Content**: No (messages are temporarily displayed, not collected)
- **Browsing History**: No
- **Usage Data**: No
- **Diagnostics Data**: No
- **Location**: No
- **Contacts**: No
- **Financial Info**: No

### Step 5: Data Sharing Declaration
- **Do you share data with third parties?**: No
- **Do you use analytics services?**: No
- **Do you use advertising networks?**: No

## Microsoft Store Categories

For app categorization:
- **Primary Category**: Developer Tools
- **Sub-category**: Development Tools / Cloud Tools
- **Age Rating**: Everyone (no age restrictions needed)

## Additional Recommendations

1. **Keep Privacy Policy Updated**: Update the "Last Updated" date whenever you make changes
2. **Version in App**: Consider adding a "Privacy Policy" link in the app that opens the policy
3. **Changelog**: If you add new features that affect privacy, update the policy
4. **GitHub Repository**: The policy is already linked in README.md

## Common Microsoft Store Questions

**Q: Does your app collect user data?**
A: No. All data is stored locally on the user's device.

**Q: Does your app transmit data to your servers?**
A: No. The app only connects directly to the user's own Azure Service Bus namespace.

**Q: Does your app use encryption?**
A: Yes. All connection strings are encrypted using Microsoft's Data Protection API.

**Q: Does your app require internet access?**
A: Yes, to connect to Azure Service Bus namespaces (user's own cloud resources).

**Q: What data does your app access?**
A: Only data from the user's Azure Service Bus namespace that they explicitly connect to.

## Testing Before Submission

Before submitting to Microsoft Store:
1. Test the privacy policy URL is accessible
2. Verify all permissions are accurately declared
3. Review the app for any analytics or tracking code
4. Ensure connection string encryption is working
5. Test local storage/file system storage functionality

## Contact for Questions

If you have questions about this privacy policy or need modifications:
- Open an issue on GitHub: https://github.com/SWojtyla/SBInspector/issues
- Review Microsoft Store Requirements: https://learn.microsoft.com/en-us/windows/apps/publish/

## Legal Disclaimer

This privacy policy template is provided as-is. While it's designed to comply with common privacy regulations including GDPR and CCPA, you should review it with legal counsel if you have specific concerns or requirements. The developer is responsible for ensuring compliance with all applicable laws and regulations.
