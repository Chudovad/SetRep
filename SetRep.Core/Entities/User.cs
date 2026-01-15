namespace SetRep.Core.Entities;

/// <summary>
/// Пользователь приложения
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор пользователя в Telegram
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания аккаунта
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Навигационное свойство: тренировки пользователя
    /// </summary>
    public ICollection<Workout> Workouts { get; set; } = [];

    /// <summary>
    /// Навигационное свойство: история веса пользователя
    /// </summary>
    public ICollection<WeightHistory> WeightHistory { get; set; } = [];

    /// <summary>
    /// Навигационное свойство: планы тренировок пользователя
    /// </summary>
    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = [];
}

