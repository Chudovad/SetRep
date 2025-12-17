using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с подходами в истории тренировок
/// </summary>
public interface IWorkoutHistorySetService
{
    /// <summary>
    /// Получить подходы для упражнения в истории тренировки
    /// </summary>
    Task<List<WorkoutHistorySet>?> GetSetsByWorkoutHistoryAndExerciseAsync(Guid workoutHistoryId, Guid exerciseId);

    /// <summary>
    /// Создать новый подход в истории тренировки
    /// </summary>
    Task<WorkoutHistorySet?> CreateSetAsync(CreateWorkoutHistorySetDto dto);

    /// <summary>
    /// Обновить подход в истории тренировки
    /// </summary>
    Task<bool> UpdateSetAsync(Guid setId, CreateWorkoutHistorySetDto dto);

    /// <summary>
    /// Удалить подход из истории тренировки
    /// </summary>
    Task<bool> DeleteSetAsync(Guid setId);
}

