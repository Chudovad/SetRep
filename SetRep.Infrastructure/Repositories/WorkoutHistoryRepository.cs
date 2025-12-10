using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с историей тренировок
/// </summary>
public class WorkoutHistoryRepository(ApplicationDbContext context) : Repository<WorkoutHistory>(context), IWorkoutHistoryRepository
{
    public async Task<IEnumerable<WorkoutHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(wh => wh.Workout)
            .Where(wh => wh.UserId == userId)
            .OrderByDescending(wh => wh.StartTime)
            .ToListAsync();
    }

    public async Task<WorkoutHistory?> GetLastWorkoutHistoryByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wh => wh.UserId == userId)
            .OrderByDescending(wh => wh.StartTime)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutHistory?> GetWithDetailsAsync(Guid workoutHistoryId)
    {
        return await _dbSet
            .Include(wh => wh.Workout)
            .Include(wh => wh.Sets)
                .ThenInclude(whs => whs.Exercise)
            .FirstOrDefaultAsync(wh => wh.Id == workoutHistoryId);
    }

    public async Task<IEnumerable<WorkoutHistory>> GetByWorkoutIdAsync(Guid workoutId)
    {
        return await _dbSet
            .Where(wh => wh.WorkoutId == workoutId)
            .OrderByDescending(wh => wh.StartTime)
            .ToListAsync();
    }
}
