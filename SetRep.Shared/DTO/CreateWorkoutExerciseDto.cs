namespace SetRep.Shared.DTO;

/// <summary>
/// DTO для создания связи между тренировкой и упражнением
/// </summary>
public class CreateWorkoutExerciseDto
{
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
    /// Таймер отдыха
    /// </summary>
    public int RestSeconds { get; set; }
}

