using SetRep.Shared.Telegram;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Реализация сервиса для хранения состояния валидации Telegram WebApp
/// </summary>
public class TelegramWebAppStateService : ITelegramWebAppStateService
{
    private TelegramWebAppValidationResult? _validationResult;

    public TelegramWebAppValidationResult? ValidationResult
    {
        get => _validationResult;
        private set
        {
            _validationResult = value;
            ValidationResultChanged?.Invoke();
        }
    }

    public event Action? ValidationResultChanged;

    public void SetValidationResult(TelegramWebAppValidationResult? result)
    {
        ValidationResult = result;
    }
}

