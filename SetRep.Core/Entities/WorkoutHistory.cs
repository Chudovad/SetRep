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
    /// Идентификатор шаблона тренировки (может быть null, если тренировка была удалена)
    /// </summary>
    public Guid? WorkoutId { get; set; }

    /// <summary>
    /// Название тренировки на момент создания (сохраняется для истории, даже если тренировка удалена)
    /// </summary>
    public string WorkoutName { get; set; } = string.Empty;

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
    /// Навигационное свойство: шаблон тренировки (может быть null, если тренировка была удалена)
    /// </summary>
    public Workout? Workout { get; set; }

    /// <summary>
    /// Навигационное свойство: пользователь
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: подходы в истории тренировки
    /// </summary>
    public ICollection<WorkoutHistorySet> Sets { get; set; } = [];
}
