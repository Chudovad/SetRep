using SetRep.Core.Enums;

namespace SetRep.Core.Entities;

/// <summary>
/// Готовый (опубликованный) план тренировок для каталога
/// </summary>
public class ReadyWorkoutPlan
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор плана тренировки
    /// </summary>
    public Guid WorkoutPlanId { get; set; }

    /// <summary>
    /// Уровень подготовки
    /// </summary>
    public ReadyPlanLevel Level { get; set; }

    /// <summary>
    /// Цель плана
    /// </summary>
    public ReadyPlanGoal Goal { get; set; }

    /// <summary>
    /// Оборудование
    /// </summary>
    public ReadyPlanEquipment Equipment { get; set; }

    /// <summary>
    /// Заметка к плану (опционально)
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Дата добавления в каталог
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: план тренировки
    /// </summary>
    public WorkoutPlan WorkoutPlan { get; set; } = null!;
}
