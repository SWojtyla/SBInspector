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
window.resizablePanels = window.resizablePanels || {};

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

    const onMouseDown = function (e) {
        isResizing = true;
        startX = e.clientX;
        startWidth = parseInt(window.getComputedStyle(leftPanel).width, 10);
        
        document.body.style.cursor = 'col-resize';
        document.body.style.userSelect = 'none';
        
        e.preventDefault();
    };

    const onMouseMove = function (e) {
        if (!isResizing) return;
        
        const dx = e.clientX - startX;
        let newWidth = startWidth + dx;
        
        // Apply min/max constraints
        if (newWidth < minWidth) newWidth = minWidth;
        if (newWidth > maxWidth) newWidth = maxWidth;
        
        leftPanel.style.width = newWidth + 'px';
        leftPanel.style.flexBasis = newWidth + 'px';
    };

    const onMouseUp = function () {
        if (isResizing) {
            isResizing = false;
            document.body.style.cursor = '';
            document.body.style.userSelect = '';
        }
    };

    resizer.addEventListener('mousedown', onMouseDown);
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);

    // Store handlers for cleanup
    window.resizablePanels[resizerId] = {
        resizer: resizer,
        onMouseDown: onMouseDown,
        onMouseMove: onMouseMove,
        onMouseUp: onMouseUp
    };
};

window.disposeResizablePanel = function (resizerId) {
    const panel = window.resizablePanels[resizerId];
    if (panel) {
        panel.resizer.removeEventListener('mousedown', panel.onMouseDown);
        document.removeEventListener('mousemove', panel.onMouseMove);
        document.removeEventListener('mouseup', panel.onMouseUp);
        delete window.resizablePanels[resizerId];
    }
};
