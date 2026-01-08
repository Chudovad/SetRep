using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы со связью тренировок и упражнений
/// </summary>
public interface IWorkoutExerciseRepository : IRepository<WorkoutExercise>
{
    /// <summary>
    /// Получить упражнения тренировки
    /// </summary>
    Task<IEnumerable<WorkoutExercise>> GetByWorkoutIdAsync(Guid workoutId);

    /// <summary>
    /// Получить упражнение тренировки с подходами
    /// </summary>
    Task<WorkoutExercise?> GetWithSetsAsync(Guid workoutExerciseId);

    /// <summary>
    /// Получить все связи тренировок и упражнений по идентификатору упражнения
    /// </summary>
    Task<IEnumerable<WorkoutExercise>> GetByExerciseIdAsync(Guid exerciseId);
}

