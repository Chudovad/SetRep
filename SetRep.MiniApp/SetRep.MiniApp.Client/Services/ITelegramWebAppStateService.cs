using SetRep.Shared.Telegram;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для хранения состояния валидации Telegram WebApp
/// </summary>
public interface ITelegramWebAppStateService
{
    /// <summary>
    /// Результат валидации
    /// </summary>
    TelegramWebAppValidationResult? ValidationResult { get; }

    /// <summary>
    /// Событие изменения результата валидации
    /// </summary>
    event Action? ValidationResultChanged;

    /// <summary>
    /// Устанавливает результат валидации
    /// </summary>
    void SetValidationResult(TelegramWebAppValidationResult? result);
}

