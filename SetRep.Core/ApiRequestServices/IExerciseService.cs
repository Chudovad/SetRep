using SetRep.Core.Entities;

namespace SetRep.MiniApp.Client.Services;

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
    /// Создать новое упражнение
    /// </summary>
    Task<Exercise?> CreateExerciseAsync(Exercise exercise);
}

