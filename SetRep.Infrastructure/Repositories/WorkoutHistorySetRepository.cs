using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с подходами в истории тренировок
/// </summary>
public class WorkoutHistorySetRepository(ApplicationDbContext context) : Repository<WorkoutHistorySet>(context), IWorkoutHistorySetRepository
{
    public async Task<IEnumerable<WorkoutHistorySet>> GetByWorkoutHistoryIdAsync(Guid workoutHistoryId)
    {
        return await _dbSet
            .Include(whs => whs.Exercise)
            .Where(whs => whs.WorkoutHistoryId == workoutHistoryId)
            .OrderBy(whs => whs.SetNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkoutHistorySet>> GetByWorkoutHistoryIdAndExerciseIdAsync(Guid workoutHistoryId, Guid exerciseId)
    {
        return await _dbSet
            .Include(whs => whs.Exercise)
            .Where(whs => whs.WorkoutHistoryId == workoutHistoryId && whs.ExerciseId == exerciseId)
            .OrderBy(whs => whs.SetNumber)
            .ToListAsync();
    }
}
