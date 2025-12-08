using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с подходами в истории тренировок
/// </summary>
public interface IWorkoutHistorySetRepository : IRepository<WorkoutHistorySet>
{
    /// <summary>
    /// Получить подходы истории тренировки
    /// </summary>
    Task<IEnumerable<WorkoutHistorySet>> GetByWorkoutHistoryIdAsync(Guid workoutHistoryId);

    /// <summary>
    /// Получить подходы упражнения в истории тренировки
    /// </summary>
    Task<IEnumerable<WorkoutHistorySet>> GetByWorkoutHistoryIdAndExerciseIdAsync(Guid workoutHistoryId, Guid exerciseId);
}
