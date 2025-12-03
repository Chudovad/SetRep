using SetRep.Shared.Telegram;

namespace SetRep.WebApp.Client.Services;

public interface ITelegramWebAppService
{
    Task<TelegramWebAppValidationResult?> ValidateAsync(string tgWebAppData);
    string? GetTgWebAppDataFromQuery();
}

