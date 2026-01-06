using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с историей тренировок
/// </summary>
public interface IWorkoutHistoryRepository : IRepository<WorkoutHistory>
{
    /// <summary>
    /// Получить историю тренировок пользователя
    /// </summary>
    Task<IEnumerable<WorkoutHistory>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить последнюю тренировку пользователя из истории
    /// </summary>
    Task<WorkoutHistory?> GetLastWorkoutHistoryByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить историю тренировки с подходами
    /// </summary>
    Task<WorkoutHistory?> GetWithDetailsAsync(Guid workoutHistoryId);

    /// <summary>
    /// Получить историю тренировок по шаблону тренировки
    /// </summary>
    Task<IEnumerable<WorkoutHistory>> GetByWorkoutIdAsync(Guid workoutId);

    /// <summary>
    /// Получить активные тренировки, начатые до указанной даты
    /// </summary>
    Task<IEnumerable<WorkoutHistory>> GetActiveWorkoutsStartedBeforeAsync(DateTime date);
}
