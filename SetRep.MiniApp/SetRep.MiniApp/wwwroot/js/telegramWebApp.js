// Скрипт для получения initData из Telegram WebApp
// Этот скрипт будет выполняться в браузере для доступа к window.Telegram.WebApp.initData

window.getTelegramWebAppData = function() {
    // Приоритет 1: Получаем из Telegram WebApp API (официальный способ)
    if (window.Telegram && window.Telegram.WebApp) {
        const webApp = window.Telegram.WebApp;
        
        // Инициализируем WebApp если еще не инициализирован
        if (typeof webApp.ready === 'function') {
            webApp.ready();
        }
        
        // Получаем initData из WebApp API
        if (webApp.initData) {
            return webApp.initData;
        }
        
        // Альтернативный способ получения initDataUnsafe
        if (webApp.initDataUnsafe) {
            // Если initDataUnsafe доступен, но initData нет, это означает что данные не подписаны
            // В этом случае нужно использовать initData из URL или параметров
        }
    }

    // Приоритет 2: Пробуем получить из URL параметров
    const urlParams = new URLSearchParams(window.location.search);
    const tgWebAppDataFromUrl = urlParams.get('tgWebAppData');
    if (tgWebAppDataFromUrl) {
        return tgWebAppDataFromUrl;
    }

    // Приоритет 3: Пробуем получить из hash параметров
    const hashParams = new URLSearchParams(window.location.hash.substring(1));
    const tgWebAppDataFromHash = hashParams.get('tgWebAppData');
    if (tgWebAppDataFromHash) {
        return tgWebAppDataFromHash;
    }

    return null;
};

// Инициализация Telegram WebApp при загрузке страницы
(function() {
    if (window.Telegram && window.Telegram.WebApp) {
        const webApp = window.Telegram.WebApp;
        
        // Расширяем приложение на весь экран
        if (typeof webApp.expand === 'function') {
            webApp.expand();
        }

        // Отключаем вертикальный свайп
        if (typeof webApp.disableVerticalSwipes === 'function') {
            webApp.disableVerticalSwipes();
        }

        if (typeof webApp.expand === 'function') {
            webApp.expand();
        }

        if (typeof webApp.requestFullscreen === 'function') {
            webApp.requestFullscreen();
        }
    }
})();

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

window.openTelegramLinkHelper = {
    isReady: function () {
        return typeof window.Telegram !== 'undefined'
            && typeof window.Telegram.WebApp !== 'undefined'
            && typeof window.Telegram.WebApp.disableVerticalSwipes === 'function';
    },

    openTelegramLink: function (link) {
        if (this.isReady()) {
            window.Telegram.WebApp.openTelegramLink(link);
        }
    },

};

window.navigationHelper = {
    goBack: function () {
        if (window.history && window.history.length > 1) {
            window.history.back();
        }
    }
};

// Тактильная отдача (вибрация) в Telegram WebApp
// Impact: heavy, light, medium, rigid, soft
// Notification: error, success, warning
// Selection: selectionChanged
window.telegramHapticHelper = {
    isReady: function () {
        return typeof window.Telegram !== 'undefined'
            && window.Telegram.WebApp
            && window.Telegram.WebApp.HapticFeedback;
    },

    impactOccurred: function (style) {
        if (this.isReady() && typeof window.Telegram.WebApp.HapticFeedback.impactOccurred === 'function') {
            window.Telegram.WebApp.HapticFeedback.impactOccurred(style || 'medium');
        }
    },

    notificationOccurred: function (type) {
        if (this.isReady() && typeof window.Telegram.WebApp.HapticFeedback.notificationOccurred === 'function') {
            window.Telegram.WebApp.HapticFeedback.notificationOccurred(type || 'success');
        }
    },

    selectionChanged: function () {
        if (this.isReady() && typeof window.Telegram.WebApp.HapticFeedback.selectionChanged === 'function') {
            window.Telegram.WebApp.HapticFeedback.selectionChanged();
        }
    }
};
