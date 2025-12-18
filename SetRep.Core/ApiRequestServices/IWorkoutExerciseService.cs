using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с упражнениями тренировки и подходами
/// </summary>
public interface IWorkoutExerciseService
{
    /// <summary>
    /// Получить упражнение тренировки с подходами
    /// </summary>
    Task<WorkoutExercise?> GetWorkoutExerciseWithSetsAsync(Guid workoutExerciseId);

    /// <summary>
    /// Получить список подходов для упражнения тренировки
    /// </summary>
    Task<List<Set>?> GetSetsByWorkoutExerciseIdAsync(Guid workoutExerciseId);

    /// <summary>
    /// Создать новый подход
    /// </summary>
    Task<Set?> CreateSetAsync(CreateSetDto createSetDto);

    /// <summary>
    /// Обновить подход
    /// </summary>
    Task<bool> UpdateSetAsync(Guid setId, CreateSetDto dto);

    /// <summary>
    /// Удалить подход
    /// </summary>
    Task<bool> DeleteSetAsync(Guid setId);

    /// <summary>
    /// Создать связь тренировки и упражнения
    /// </summary>
    Task<WorkoutExercise?> CreateWorkoutExerciseAsync(CreateWorkoutExerciseDto dto);
}

