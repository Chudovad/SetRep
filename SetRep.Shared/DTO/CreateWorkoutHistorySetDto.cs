namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для создания нового подхода в истории тренировки
/// </summary>
public class CreateWorkoutHistorySetDto
{
    /// <summary>
    /// Идентификатор истории тренировки
    /// </summary>
    public Guid WorkoutHistoryId { get; set; }

    /// <summary>
    /// Идентификатор упражнения
    /// </summary>
    public Guid ExerciseId { get; set; }

    /// <summary>
    /// Номер подхода (1, 2, 3...)
    /// </summary>
    public int SetNumber { get; set; }

    /// <summary>
    /// Количество повторений
    /// </summary>
    public int Reps { get; set; }

    /// <summary>
    /// Вес (в единицах измерения упражнения)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Длительность подхода в секундах (для кардио или изометрических упражнений)
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Расстояние в метрах (для бега, плавания и т.д.)
    /// </summary>
    public decimal? DistanceMeters { get; set; }

    /// <summary>
    /// Отдых после подхода в секундах
    /// </summary>
    public int? RestSeconds { get; set; }

    /// <summary>
    /// Отметка о выполнении подхода (может быть не выполнен)
    /// </summary>
    public bool IsCompleted { get; set; } = true;

    /// <summary>
    /// Заметки к подходу
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Дата и время выполнения подхода
    /// </summary>
    public DateTime CompletedAt { get; set; }
}
