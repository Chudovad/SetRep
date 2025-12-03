using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с подходами
/// </summary>
public class SetRepository(ApplicationDbContext context) : Repository<Set>(context), ISetRepository
{
    public async Task<IEnumerable<Set>> GetByWorkoutExerciseIdAsync(Guid workoutExerciseId)
    {
        return await _dbSet
            .Where(s => s.WorkoutExerciseId == workoutExerciseId)
            .OrderBy(s => s.SetNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<Set>> GetByWorkoutIdAsync(Guid workoutId)
    {
        return await _dbSet
            .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we!.Exercise)
            .Include(s => s.WorkoutExercise)
                .ThenInclude(we => we!.Workout)
            .Where(s => s.WorkoutExercise != null && s.WorkoutExercise.WorkoutId == workoutId)
            .OrderBy(s => s.WorkoutExercise!.Order)
            .ThenBy(s => s.SetNumber)
            .ToListAsync();
    }
}

