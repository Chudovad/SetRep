namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для создания нового плана тренировки
/// </summary>
public class CreateWorkoutPlanDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Название плана тренировки
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
