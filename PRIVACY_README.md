# Privacy Policy Files - Quick Reference

## Overview
This directory contains comprehensive privacy policy documentation for SBInspector, ready for Microsoft Store submission.

## Files Included

### 1. PRIVACY_POLICY.md
- **Format**: Markdown
- **Lines**: 129
- **Use for**: 
  - GitHub repository display
  - Website hosting
  - Detailed legal reference
- **Content**: Complete privacy policy with all sections

### 2. PRIVACY_POLICY_SHORT.md
- **Format**: Markdown
- **Lines**: 40
- **Use for**:
  - Quick reference
  - App store descriptions
  - Microsoft Store submission form
- **Special**: Contains specific "Privacy Statement for Microsoft Store" at bottom

### 3. PRIVACY_POLICY.txt
- **Format**: Plain text
- **Lines**: 83
- **Use for**:
  - Copy-paste into text forms
  - Email templates
  - Plain text requirements
- **Content**: Same as full policy but formatted as plain text

### 4. MICROSOFT_STORE_SUBMISSION_GUIDE.md
- **Format**: Markdown
- **Lines**: 118
- **Use for**:
  - Step-by-step submission guide
  - Answering common questions
  - Understanding permission declarations
- **Content**: Complete walkthrough of Microsoft Store submission process

## Quick Copy-Paste for Microsoft Store

**Privacy Statement (use this in the Microsoft Store submission form):**

```
SBInspector does not collect, use, or share any user data. All data (Azure Service Bus connection strings and message templates) is stored locally on the user's device in encrypted form. The application connects directly to the user's Azure Service Bus namespace and does not transmit any data to third parties or our servers. No analytics, tracking, or advertising services are used.
```

**Privacy Policy URL Options:**
1. GitHub Raw: `https://raw.githubusercontent.com/SWojtyla/SBInspector/main/PRIVACY_POLICY.md`
2. GitHub Pages (if enabled): `https://swojtyla.github.io/SBInspector/PRIVACY_POLICY.html`
3. Your own website (copy content from PRIVACY_POLICY.md)

## Key Points

✅ **No data collection** - App doesn't collect any user data  
✅ **Local storage only** - All data stored on user's device  
✅ **Encryption** - Connection strings encrypted with Microsoft Data Protection API  
✅ **No third parties** - No data shared with anyone  
✅ **No tracking** - No analytics, telemetry, or advertising  
✅ **Open source** - Full code available for review  

## Data Declarations for Store

When filling out Microsoft Store forms:

**Personal Information**: ❌ No  
**User Content**: ❌ No  
**Browsing History**: ❌ No  
**Usage Data**: ❌ No  
**Diagnostics**: ❌ No  
**Location**: ❌ No  
**Financial Info**: ❌ No  

**Share with third parties**: ❌ No  
**Use analytics**: ❌ No  
**Use advertising**: ❌ No  

## Permissions Required

**Desktop App:**
- File System Access (to save encrypted connection strings locally)
- Network Access (to connect to Azure Service Bus)

**Web App:**
- Local Storage (to save encrypted connection strings in browser)
- Network Access (to connect to Azure Service Bus)

## Next Steps

1. Read `MICROSOFT_STORE_SUBMISSION_GUIDE.md` for detailed instructions
2. Copy the privacy statement from `PRIVACY_POLICY_SHORT.md`
3. Choose a privacy policy URL option
4. Complete the Microsoft Store submission form
5. Test the app before submission

## Questions?

Open an issue: https://github.com/SWojtyla/SBInspector/issues

## Legal Note

This privacy policy is designed to comply with GDPR, CCPA, and Microsoft Store requirements. Review with legal counsel if you have specific concerns.

---

**Created**: November 7, 2025  
**For**: Microsoft Store Submission  
**Repository**: https://github.com/SWojtyla/SBInspector
