using SetRep.Core.Entities;

namespace SetRep.Core.ApiRequestServices;

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
    /// Получить историю тренировок пользователя
    /// </summary>
    Task<List<WorkoutHistory>?> GetWorkoutHistoryByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить детали истории тренировки
    /// </summary>
    Task<WorkoutHistory?> GetWorkoutHistoryDetailsAsync(Guid workoutHistoryId);

    /// <summary>
    /// Удалить запись истории тренировки
    /// </summary>
    Task<bool> DeleteWorkoutHistoryAsync(Guid workoutHistoryId);
}
