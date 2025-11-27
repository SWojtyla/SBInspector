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

// Resizable panel functionality
window.initResizablePanel = function (resizerId, leftPanelId, minWidth, maxWidth) {
    const resizer = document.getElementById(resizerId);
    const leftPanel = document.getElementById(leftPanelId);
    
    if (!resizer || !leftPanel) {
        console.warn('Resizer or left panel not found');
        return;
    }

    let isResizing = false;
    let startX = 0;
    let startWidth = 0;

    resizer.addEventListener('mousedown', function (e) {
        isResizing = true;
        startX = e.clientX;
        startWidth = parseInt(window.getComputedStyle(leftPanel).width, 10);
        
        document.body.style.cursor = 'col-resize';
        document.body.style.userSelect = 'none';
        
        e.preventDefault();
    });

    document.addEventListener('mousemove', function (e) {
        if (!isResizing) return;
        
        const dx = e.clientX - startX;
        let newWidth = startWidth + dx;
        
        // Apply min/max constraints
        if (newWidth < minWidth) newWidth = minWidth;
        if (newWidth > maxWidth) newWidth = maxWidth;
        
        leftPanel.style.width = newWidth + 'px';
        leftPanel.style.flexBasis = newWidth + 'px';
    });

    document.addEventListener('mouseup', function () {
        if (isResizing) {
            isResizing = false;
            document.body.style.cursor = '';
            document.body.style.userSelect = '';
        }
    });
};

window.disposeResizablePanel = function (resizerId) {
    const resizer = document.getElementById(resizerId);
    if (resizer) {
        // Remove event listeners by cloning and replacing the element
        const newResizer = resizer.cloneNode(true);
        resizer.parentNode.replaceChild(newResizer, resizer);
    }
};
