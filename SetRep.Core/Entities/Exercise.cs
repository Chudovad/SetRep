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
    /// Гифка упражнения
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Главная картинка упражнения (для отображения в карточках)
    /// </summary>
    public string? MainImage { get; set; }

    /// <summary>
    /// Ссылка на видео выполнения упражнения
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// Описание упражнения
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Группа мышц
    /// </summary>
    public MuscleGroup? MuscleGroup { get; set; } = Enums.MuscleGroup.Other;

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