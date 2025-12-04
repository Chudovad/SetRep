using SetRep.Shared.Telegram;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с Telegram WebApp данными
/// </summary>
public interface ITelegramWebAppService
{
    /// <summary>
    /// Получает параметры tgWebAppData из URL
    /// </summary>
    Task<string?> GetTelegramWebAppDataAsync();

    /// <summary>
    /// Валидирует данные Telegram WebApp через API
    /// </summary>
    Task<TelegramWebAppValidationResult> ValidateAsync(string tgWebAppData);

    /// <summary>
    /// Получает и валидирует данные автоматически
    /// </summary>
    Task<TelegramWebAppValidationResult> GetAndValidateAsync();
}

