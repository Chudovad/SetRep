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

window.telegramBackButtonHelper = {
    isReady: function () {
        return typeof window.Telegram !== 'undefined'
            && typeof window.Telegram.WebApp !== 'undefined'
            && typeof window.Telegram.WebApp.BackButton !== 'undefined';
    },

    show: function () {
        if (this.isReady()) {
            window.Telegram.WebApp.BackButton.show();
        }
    },

    hide: function () {
        if (this.isReady()) {
            window.Telegram.WebApp.BackButton.hide();
        }
    },

    onClick: function (dotnetHelper) {
        if (this.isReady()) {
            window.Telegram.WebApp.BackButton.onClick(function () {
                dotnetHelper.invokeMethodAsync('OnBackButtonClick');
            });
        }
    }
};