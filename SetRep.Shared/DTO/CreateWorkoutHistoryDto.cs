namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для создания новой записи истории тренировки
/// </summary>
public class CreateWorkoutHistoryDto
{
    /// <summary>
    /// Идентификатор шаблона тренировки
    /// </summary>
    public Guid WorkoutId { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

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
