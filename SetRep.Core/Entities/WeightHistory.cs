namespace SetRep.Core.Entities;

/// <summary>
/// История веса пользователя
/// </summary>
public class WeightHistory
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Вес в килограммах
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Дата измерения веса
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Заметки к записи
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
}

