namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Вспомогательный класс для ожидания завершения инициализации приложения
/// </summary>
public static class InitializationHelper
{
    /// <summary>
    /// Ожидает завершения инициализации приложения с таймаутом
    /// </summary>
    /// <param name="appInitializationService">Сервис инициализации приложения</param>
    /// <param name="timeoutSeconds">Таймаут в секундах (по умолчанию 10 секунд)</param>
    /// <param name="checkIntervalMs">Интервал проверки в миллисекундах (по умолчанию 100 мс)</param>
    /// <returns>Задача, которая завершается после окончания инициализации или таймаута</returns>
    public static async Task WaitForInitializationAsync(
        IAppInitializationService appInitializationService,
        int timeoutSeconds = 10,
        int checkIntervalMs = 100)
    {
        if (!appInitializationService.IsInitializing)
        {
            return;
        }

        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        var startTime = DateTime.UtcNow;
        var checkInterval = TimeSpan.FromMilliseconds(checkIntervalMs);

        while (appInitializationService.IsInitializing)
        {
            if (DateTime.UtcNow - startTime > timeout)
            {
                break;
            }

            await Task.Delay(checkInterval);
        }
    }
}

