using SetRep.Core.Enums;

namespace SetRep.Core.Entities;

/// <summary>
/// Упражнение
/// </summary>
public class Exercise
{
    /// <summary>
    /// Уникальный идентификатор упражнения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название упражнения
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание упражнения
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Группа мышц
    /// </summary>
    public MuscleGroup? MuscleGroup { get; set; }

    /// <summary>
    /// Единица измерения веса (например: "кг", "lbs")
    /// </summary>
    public string WeightUnit { get; set; } = "кг";

    /// <summary>
    /// Является ли упражнение системным (доступно всем пользователям)
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего упражнение (null для системных)
    /// </summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Дата создания упражнения
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: использование упражнения в тренировках
    /// </summary>
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];
}