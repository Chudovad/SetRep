using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с тренировками
/// </summary>
public interface IWorkoutService
{
    /// <summary>
    /// Получить все тренировки для конкретного пользователя
    /// </summary>
    Task<List<Workout>?> GetWorkoutsAsync(Guid userId);

    /// <summary>
    /// Получить детали тренировки с упражнениями
    /// </summary>
    Task<Workout?> GetWorkoutDetailsAsync(Guid workoutId);

    /// <summary>
    /// Создать новую тренировку
    /// </summary>
    Task<Workout?> CreateWorkoutAsync(CreateWorkoutDto dto);

    /// <summary>
    /// Получить историю тренировок для шаблона тренировки
    /// </summary>
    Task<List<WorkoutHistory>?> GetWorkoutHistoryByWorkoutIdAsync(Guid workoutId);

    /// <summary>
    /// Удалить тренировку
    /// </summary>
    Task<bool> DeleteWorkoutAsync(Guid workoutId);

    /// <summary>
    /// Обновить тренировку
    /// </summary>
    Task<bool> UpdateWorkoutAsync(Guid workoutId, CreateWorkoutDto dto);

    /// <summary>
    /// Обновить порядок упражнения в тренировке
    /// </summary>
    Task<bool> UpdateWorkoutExerciseOrderAsync(Guid workoutExerciseId, int newOrder);
}

