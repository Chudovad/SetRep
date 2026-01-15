using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с планами тренировок
/// </summary>
public interface IWorkoutPlanRepository : IRepository<WorkoutPlan>
{
    /// <summary>
    /// Получить планы тренировок пользователя
    /// </summary>
    Task<IEnumerable<WorkoutPlan>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить план тренировки с тренировками
    /// </summary>
    Task<WorkoutPlan?> GetWithWorkoutsAsync(Guid workoutPlanId);
}
