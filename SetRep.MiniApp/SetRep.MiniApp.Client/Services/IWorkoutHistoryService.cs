using SetRep.Core.Entities;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с историей тренировок
/// </summary>
public interface IWorkoutHistoryService
{
    /// <summary>
    /// Начать тренировку - создает запись в WorkoutHistory
    /// </summary>
    Task<WorkoutHistory?> StartWorkoutAsync(Guid workoutId, Guid userId);

    /// <summary>
    /// Завершить тренировку - обновляет EndTime
    /// </summary>
    Task<bool> EndWorkoutAsync(Guid workoutHistoryId);

    /// <summary>
    /// Получить активную тренировку пользователя
    /// </summary>
    Task<WorkoutHistory?> GetActiveWorkoutAsync(Guid userId);

    /// <summary>
    /// Копировать подходы из тренировки в историю
    /// </summary>
    Task<bool> CopySetsFromWorkoutAsync(Guid workoutHistoryId, Guid workoutId);

    /// <summary>
    /// Получить текущее время тренировки
    /// </summary>
    TimeSpan GetWorkoutDuration(WorkoutHistory workoutHistory);

    /// <summary>
    /// Событие обновления времени тренировки
    /// </summary>
    event Action<TimeSpan>? DurationUpdated;
}
