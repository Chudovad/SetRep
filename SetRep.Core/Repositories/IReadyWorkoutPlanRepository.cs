using SetRep.Core.Entities;
using SetRep.Core.Enums;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с готовыми планами тренировок
/// </summary>
public interface IReadyWorkoutPlanRepository : IRepository<ReadyWorkoutPlan>
{
    /// <summary>
    /// Получить готовые планы с фильтрами (с включённым WorkoutPlan)
    /// </summary>
    Task<IEnumerable<ReadyWorkoutPlan>> GetFilteredAsync(
        ReadyPlanLevel? level,
        ReadyPlanGoal? goal,
        ReadyPlanEquipment? equipment);

    /// <summary>
    /// Получить запись готового плана по идентификатору плана тренировки
    /// </summary>
    Task<ReadyWorkoutPlan?> GetByWorkoutPlanIdAsync(Guid workoutPlanId);

    /// <summary>
    /// Проверить, есть ли уже запись для данного плана
    /// </summary>
    Task<bool> ExistsByWorkoutPlanIdAsync(Guid workoutPlanId);
}
