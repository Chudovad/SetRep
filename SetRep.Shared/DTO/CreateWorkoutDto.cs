namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для создания новой тренировки
/// </summary>
public class CreateWorkoutDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Название тренировки
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Заметки к тренировке
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Идентификатор плана тренировки (опционально)
    /// </summary>
    public Guid? WorkoutPlanId { get; set; }
}

