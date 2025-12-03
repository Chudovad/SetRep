using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с подходами
/// </summary>
public interface ISetRepository : IRepository<Set>
{
    /// <summary>
    /// Получить подходы упражнения в тренировке
    /// </summary>
    Task<IEnumerable<Set>> GetByWorkoutExerciseIdAsync(Guid workoutExerciseId);

    /// <summary>
    /// Получить подходы тренировки
    /// </summary>
    Task<IEnumerable<Set>> GetByWorkoutIdAsync(Guid workoutId);
}

