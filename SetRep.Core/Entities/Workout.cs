namespace SetRep.Core.Entities;

/// <summary>
/// Тренировка
/// </summary>
public class Workout
{
    /// <summary>
    /// Уникальный идентификатор тренировки
    /// </summary>
    public Guid Id { get; set; }

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
    /// Навигационное свойство: упражнения в тренировке
    /// </summary>
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];
}

