namespace SetRep.Core.Entities;

/// <summary>
/// План тренировки
/// </summary>
public class WorkoutPlan
{
    /// <summary>
    /// Уникальный идентификатор плана тренировки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Название плана тренировки
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: пользователь
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: тренировки в плане
    /// </summary>
    public ICollection<Workout> Workouts { get; set; } = [];
}
