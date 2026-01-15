using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с планами тренировок
/// </summary>
public class WorkoutPlanRepository(ApplicationDbContext context) : Repository<WorkoutPlan>(context), IWorkoutPlanRepository
{
    public async Task<IEnumerable<WorkoutPlan>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(wp => wp.UserId == userId)
            .ToListAsync();
    }

    public async Task<WorkoutPlan?> GetWithWorkoutsAsync(Guid workoutPlanId)
    {
        return await _dbSet
            .Include(wp => wp.Workouts)
            .ThenInclude(w => w.WorkoutExercises)
            .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(wp => wp.Id == workoutPlanId);
    }
}
