using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с тренировками
/// </summary>
public interface IWorkoutService
{
    /// <summary>
    /// Получить все тренировки
    /// </summary>
    Task<List<Workout>?> GetWorkoutsAsync();

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
}

