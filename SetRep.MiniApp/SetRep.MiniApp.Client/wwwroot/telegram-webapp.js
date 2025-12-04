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
        
        // Включаем закрытие через кнопку назад
        if (typeof webApp.enableClosingConfirmation === 'function') {
            webApp.enableClosingConfirmation();
        }
        
        // Устанавливаем цвет темы
        if (typeof webApp.setHeaderColor === 'function') {
            webApp.setHeaderColor('#7e6fff');
        }
        
        if (typeof webApp.setBackgroundColor === 'function') {
            webApp.setBackgroundColor('#1a1a27');
        }
    }
})();

