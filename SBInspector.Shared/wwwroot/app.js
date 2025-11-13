// File download function
window.downloadFile = function (filename, content) {
    const blob = new Blob([content], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
};

// Folder picker function using File System Access API
window.pickFolder = async function () {
    try {
        // Check if the File System Access API is supported
        if (!window.showDirectoryPicker) {
            console.warn('File System Access API is not supported in this browser');
            return null;
        }

        // Show the directory picker
        const directoryHandle = await window.showDirectoryPicker();
        
        // Return the directory name/path
        // Note: For security reasons, browsers don't expose the full file system path
        // We return the name of the directory as that's all we can reliably get
        return directoryHandle.name;
    } catch (error) {
        // User cancelled or error occurred
        console.log('Folder picker cancelled or error:', error);
        return null;
    }
};
