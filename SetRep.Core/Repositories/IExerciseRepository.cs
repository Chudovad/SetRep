using SetRep.Core.Entities;
using SetRep.Core.Enums;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с упражнениями
/// </summary>
public interface IExerciseRepository : IRepository<Exercise>
{
    /// <summary>
    /// Получить системные упражнения
    /// </summary>
    Task<IEnumerable<Exercise>> GetSystemExercisesAsync();

    /// <summary>
    /// Получить упражнения пользователя
    /// </summary>
    Task<IEnumerable<Exercise>> GetUserExercisesAsync(Guid userId);

    /// <summary>
    /// Получить упражнения по группе мышц
    /// </summary>
    Task<IEnumerable<Exercise>> GetByMuscleGroupAsync(MuscleGroup muscleGroup);

    /// <summary>
    /// Поиск упражнений по названию
    /// </summary>
    Task<IEnumerable<Exercise>> SearchByNameAsync(string searchTerm);
}

