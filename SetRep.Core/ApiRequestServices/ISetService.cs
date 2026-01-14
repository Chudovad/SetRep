using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с подходами тренировки
/// </summary>
public interface ISetService
{
    /// <summary>
    /// Получить подходы упражнения в тренировке
    /// </summary>
    Task<List<Set>?> GetSetsByWorkoutExerciseIdAsync(Guid workoutExerciseId);

    /// <summary>
    /// Получить подходы тренировки
    /// </summary>
    Task<List<Set>?> GetSetsByWorkoutIdAsync(Guid workoutId);

    /// <summary>
    /// Обновить подход
    /// </summary>
    Task<bool> UpdateSetAsync(Guid setId, CreateSetDto dto);
}
