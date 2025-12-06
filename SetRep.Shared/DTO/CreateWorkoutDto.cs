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
    /// Дата и время начала тренировки
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Дата и время окончания тренировки
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Заметки к тренировке
    /// </summary>
    public string? Notes { get; set; }
}

