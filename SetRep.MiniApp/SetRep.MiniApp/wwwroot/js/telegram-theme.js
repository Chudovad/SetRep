window.telegramThemeHelper = {
    isReady: function () {
        return typeof window.Telegram !== 'undefined'
            && typeof window.Telegram.WebApp !== 'undefined'
            && window.Telegram.WebApp.ready !== undefined;
    },

    getTheme: function () {
        if (this.isReady()) {
            return window.Telegram.WebApp.colorScheme || 'dark';
        }
        return 'dark';
    },

    setThemeCallback: function (dotnetHelper) {
        if (this.isReady()) {
            window.Telegram.WebApp.onEvent('themeChanged', function () {
                const theme = window.Telegram.WebApp.colorScheme;
                dotnetHelper.invokeMethodAsync('OnTelegramThemeChanged', theme);
            });
        }
    },

    getThemeParams: function () {
        if (this.isReady()) {
            return window.Telegram.WebApp.themeParams;
        }
        return null;
    }
};