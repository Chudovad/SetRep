using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с готовыми планами тренировок
/// </summary>
public interface IReadyWorkoutPlanService
{
    /// <summary>
    /// Получить список готовых планов с опциональными фильтрами
    /// </summary>
    Task<List<ReadyWorkoutPlan>?> GetReadyPlansAsync(
        ReadyPlanLevel? level = null,
        ReadyPlanGoal? goal = null,
        ReadyPlanEquipment? equipment = null);

    /// <summary>
    /// Получить запись готового плана по идентификатору плана тренировки
    /// </summary>
    Task<ReadyWorkoutPlan?> GetReadyPlanByWorkoutPlanIdAsync(Guid planId);

    /// <summary>
    /// Добавить план в список готовых
    /// </summary>
    Task<ReadyWorkoutPlan?> AddReadyPlanAsync(CreateReadyWorkoutPlanDto dto);
}
