window.themeHelper = {
    setTheme: function(isDarkMode) {
        if (isDarkMode) {
            document.body.classList.remove('light-theme');
            document.body.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
            document.body.classList.add('light-theme');
        }
    },
    
    getTheme: function() {
        return document.body.classList.contains('dark-theme');
    }
};
