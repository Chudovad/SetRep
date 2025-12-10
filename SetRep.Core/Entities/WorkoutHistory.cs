namespace SetRep.Core.Entities;

/// <summary>
/// История выполненных тренировок
/// </summary>
public class WorkoutHistory
{
    /// <summary>
    /// Уникальный идентификатор записи истории тренировки
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: шаблон тренировки
    /// </summary>
    public Workout Workout { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: пользователь
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: подходы в истории тренировки
    /// </summary>
    public ICollection<WorkoutHistorySet> Sets { get; set; } = [];
}
