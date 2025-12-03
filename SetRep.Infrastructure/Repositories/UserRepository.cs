using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с пользователями
/// </summary>
public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByTelegramIdAsync(Guid telegramId)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }
}

