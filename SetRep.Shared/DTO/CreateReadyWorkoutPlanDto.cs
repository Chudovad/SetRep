namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для добавления плана в список готовых
/// </summary>
public class CreateReadyWorkoutPlanDto
{
    /// <summary>
    /// Идентификатор плана тренировки
    /// </summary>
    public Guid WorkoutPlanId { get; set; }

    /// <summary>
    /// Уровень подготовки (ReadyPlanLevel enum value)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Цель плана (ReadyPlanGoal enum value)
    /// </summary>
    public int Goal { get; set; }

    /// <summary>
    /// Оборудование (ReadyPlanEquipment enum value)
    /// </summary>
    public int Equipment { get; set; }

    /// <summary>
    /// Заметка к плану (опционально)
    /// </summary>
    public string? Note { get; set; }
}
