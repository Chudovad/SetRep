using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с тренировками
/// </summary>
public interface IWorkoutRepository : IRepository<Workout>
{
    /// <summary>
    /// Получить тренировки пользователя
    /// </summary>
    Task<IEnumerable<Workout>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить последнюю тренировку пользователя
    /// </summary>
    Task<Workout?> GetLastWorkoutByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить тренировку с упражнениями и подходами
    /// </summary>
    Task<Workout?> GetWithDetailsAsync(Guid workoutId);
}

