using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с пользователями
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    Task<User?> GetByTelegramIdAsync(long telegramId);

    /// <summary>
    /// Получить пользователя по имени пользователя
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);
}

