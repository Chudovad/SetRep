using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;
using static SetRep.Core.Constants;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с историей измерений
/// </summary>
public class MeasurementsHistoryRepository(ApplicationDbContext context) : Repository<MeasurementsHistory>(context), IMeasurementsHistoryRepository
{
    public async Task<IEnumerable<MeasurementsHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId && wh.Notes != AllEntriesNotes)
            .OrderByDescending(wh => wh.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<MeasurementsHistory>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId && wh.Notes != AllEntriesNotes && wh.Date >= startDate && wh.Date <= endDate)
            .OrderByDescending(wh => wh.Date)
            .ToListAsync();
    }

    public async Task<MeasurementsHistory?> GetLastByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId)
            .OrderByDescending(wh => wh.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<MeasurementsHistory?> GetByUserIdAndNotesAsync(Guid userId, string notes)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId && wh.Notes == notes)
            .FirstOrDefaultAsync();
    }
}

