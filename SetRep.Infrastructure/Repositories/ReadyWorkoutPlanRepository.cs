using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с готовыми планами тренировок
/// </summary>
public class ReadyWorkoutPlanRepository(ApplicationDbContext context)
    : Repository<ReadyWorkoutPlan>(context), IReadyWorkoutPlanRepository
{
    public async Task<IEnumerable<ReadyWorkoutPlan>> GetFilteredAsync(
        ReadyPlanLevel? level,
        ReadyPlanGoal? goal,
        ReadyPlanEquipment? equipment)
    {
        var query = _dbSet.Include(r => r.WorkoutPlan).AsQueryable();

        if (level.HasValue)
            query = query.Where(r => r.Level == level.Value);
        if (goal.HasValue)
            query = query.Where(r => r.Goal == goal.Value);
        if (equipment.HasValue)
            query = query.Where(r => r.Equipment == equipment.Value);

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<ReadyWorkoutPlan?> GetByWorkoutPlanIdAsync(Guid workoutPlanId)
    {
        return await _dbSet
            .Include(r => r.WorkoutPlan)
            .FirstOrDefaultAsync(r => r.WorkoutPlanId == workoutPlanId);
    }

    public async Task<bool> ExistsByWorkoutPlanIdAsync(Guid workoutPlanId)
    {
        return await _dbSet.AnyAsync(r => r.WorkoutPlanId == workoutPlanId);
    }
}
