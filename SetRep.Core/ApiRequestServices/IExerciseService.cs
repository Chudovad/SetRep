using SetRep.Core.Entities;
using SetRep.Core.Enums;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с упражнениями
/// </summary>
public interface IExerciseService
{
    /// <summary>
    /// Получить все упражнения
    /// </summary>
    Task<List<Exercise>?> GetExercisesAsync();

    /// <summary>
    /// Получить упражнение по идентификатору
    /// </summary>
    Task<Exercise?> GetExerciseByIdAsync(Guid exerciseId);

    /// <summary>
    /// Получить упражнения по группе мышц
    /// </summary>
    Task<List<Exercise>?> GetExercisesByMuscleGroupAsync(MuscleGroup muscleGroup);

    /// <summary>
    /// Создать новое упражнение
    /// </summary>
    Task<Exercise?> CreateExerciseAsync(Exercise exercise);

    /// <summary>
    /// Обновить упражнение
    /// </summary>
    Task<bool> UpdateExerciseAsync(Exercise exercise);

    /// <summary>
    /// Удалить упражнение
    /// </summary>
    Task<bool> DeleteExerciseAsync(Guid exerciseId);
}

