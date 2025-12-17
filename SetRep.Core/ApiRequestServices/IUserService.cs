using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    Task<UserDto?> GetUserByTelegramIdAsync(long telegramId);
}

