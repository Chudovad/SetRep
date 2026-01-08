using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы со связью тренировок и упражнений
/// </summary>
public class WorkoutExerciseRepository(ApplicationDbContext context) : Repository<WorkoutExercise>(context), IWorkoutExerciseRepository
{
    public async Task<IEnumerable<WorkoutExercise>> GetByWorkoutIdAsync(Guid workoutId)
    {
        return await _dbSet
            .Where(we => we.WorkoutId == workoutId)
            .Include(we => we.Exercise)
            .Include(we => we.Sets)
            .OrderBy(we => we.Order)
            .ToListAsync();
    }

    public async Task<WorkoutExercise?> GetWithSetsAsync(Guid workoutExerciseId)
    {
        return await _dbSet
            .Include(we => we.Exercise)
            .Include(we => we.Sets.OrderBy(s => s.SetNumber))
            .FirstOrDefaultAsync(we => we.Id == workoutExerciseId);
    }

    public async Task<IEnumerable<WorkoutExercise>> GetByExerciseIdAsync(Guid exerciseId)
    {
        return await _dbSet
            .Where(we => we.ExerciseId == exerciseId)
            .ToListAsync();
    }
}

