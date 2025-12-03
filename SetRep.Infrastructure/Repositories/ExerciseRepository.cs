using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;

namespace SetRep.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с упражнениями
/// </summary>
public class ExerciseRepository(ApplicationDbContext context) : Repository<Exercise>(context), IExerciseRepository
{
    public async Task<IEnumerable<Exercise>> GetSystemExercisesAsync()
    {
        return await _dbSet
            .Where(e => e.IsSystem)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> GetUserExercisesAsync(Guid userId)
    {
        return await _dbSet
            .Where(e => !e.IsSystem && e.CreatedByUserId == userId)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> GetByMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        return await _dbSet
            .Where(e => e.MuscleGroup == muscleGroup)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exercise>> SearchByNameAsync(string searchTerm)
    {
        return await _dbSet
            .Where(e => e.Name.Contains(searchTerm))
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}

