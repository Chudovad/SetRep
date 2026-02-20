namespace SetRep.Core.Entities;

/// <summary>
/// Связь между тренировкой и упражнением
/// </summary>
public class WorkoutExercise
{
    /// <summary>
    /// Уникальный идентификатор связи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор тренировки
    /// </summary>
    public Guid WorkoutId { get; set; }

    /// <summary>
    /// Идентификатор упражнения
    /// </summary>
    public Guid ExerciseId { get; set; }

    /// <summary>
    /// Порядок выполнения упражнения в тренировке
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Заметки к конкретному упражнению в этой тренировке
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: тренировка
    /// </summary>
    public Workout Workout { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: упражнение
    /// </summary>
    public Exercise Exercise { get; set; } = null!;

    /// <summary>
    /// Навигационное свойство: подходы для этого упражнения в тренировке
    /// </summary>
    public ICollection<Set> Sets { get; set; } = [];

    /// <summary>
    /// Время отдыха после подхода в секундах (по умолчанию 60)
    /// </summary>
    public int RestSeconds { get; set; } = 60;
}

