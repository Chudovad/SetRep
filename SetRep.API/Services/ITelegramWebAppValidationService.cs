using SetRep.Shared.Telegram;

namespace SetRep.API.Services;

public interface ITelegramWebAppValidationService
{
    TelegramWebAppValidationResult Validate(string tgWebAppData, string botToken);
}

