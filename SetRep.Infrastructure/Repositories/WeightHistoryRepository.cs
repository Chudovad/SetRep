using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с историей веса
/// </summary>
public class WeightHistoryRepository(ApplicationDbContext context) : Repository<WeightHistory>(context), IWeightHistoryRepository
{
    public async Task<IEnumerable<WeightHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId)
            .OrderByDescending(wh => wh.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeightHistory>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId && wh.Date >= startDate && wh.Date <= endDate)
            .OrderByDescending(wh => wh.Date)
            .ToListAsync();
    }

    public async Task<WeightHistory?> GetLastByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId)
            .OrderByDescending(wh => wh.Date)
            .FirstOrDefaultAsync();
    }
}

