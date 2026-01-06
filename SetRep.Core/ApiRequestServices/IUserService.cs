using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    Task<UserDto?> GetUserByTelegramIdAsync(long telegramId);

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    Task<UserDto?> CreateUserAsync(UserDto user);
}

