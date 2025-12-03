using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с тренировками
/// </summary>
public class WorkoutRepository(ApplicationDbContext context) : Repository<Workout>(context), IWorkoutRepository
{
    public async Task<IEnumerable<Workout>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workout>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(w => w.UserId == userId && w.StartTime >= startDate && w.StartTime <= endDate)
            .OrderByDescending(w => w.StartTime)
            .ToListAsync();
    }

    public async Task<Workout?> GetLastWorkoutByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.StartTime)
            .FirstOrDefaultAsync();
    }

    public async Task<Workout?> GetWithDetailsAsync(Guid workoutId)
    {
        return await _dbSet
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .FirstOrDefaultAsync(w => w.Id == workoutId);
    }
}

