using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с планами тренировок
/// </summary>
public interface IWorkoutPlanService
{
    /// <summary>
    /// Получить все планы тренировок для конкретного пользователя
    /// </summary>
    Task<List<WorkoutPlan>?> GetWorkoutPlansAsync(Guid userId);

    /// <summary>
    /// Получить план тренировки по идентификатору
    /// </summary>
    Task<WorkoutPlan?> GetWorkoutPlanByIdAsync(Guid workoutPlanId);

    /// <summary>
    /// Получить план тренировки с деталями (тренировками)
    /// </summary>
    Task<WorkoutPlan?> GetWorkoutPlanDetailsAsync(Guid workoutPlanId);

    /// <summary>
    /// Создать новый план тренировки
    /// </summary>
    Task<WorkoutPlan?> CreateWorkoutPlanAsync(CreateWorkoutPlanDto dto);

    /// <summary>
    /// Обновить план тренировки
    /// </summary>
    Task<bool> UpdateWorkoutPlanAsync(Guid workoutPlanId, CreateWorkoutPlanDto dto);

    /// <summary>
    /// Удалить план тренировки
    /// </summary>
    Task<bool> DeleteWorkoutPlanAsync(Guid workoutPlanId);
}
