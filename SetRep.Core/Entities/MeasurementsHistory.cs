namespace SetRep.Core.Entities;

/// <summary>
/// История измерений пользователя
/// </summary>
public class MeasurementsHistory
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
    /// Дата измерения
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// URL изображения прогресса
    /// </summary>
    public string? ProgressImageUrl { get; set; }

    /// <summary>
    /// Масса тела в килограммах
    /// </summary>
    public decimal? BodyWeight { get; set; }

    /// <summary>
    /// Обхват талии в сантиметрах
    /// </summary>
    public decimal? Waist { get; set; }

    /// <summary>
    /// Содержание жира в процентах
    /// </summary>
    public decimal? BodyFatPercentage { get; set; }

    /// <summary>
    /// Сухая мышечная масса в килограммах
    /// </summary>
    public decimal? LeanBodyMass { get; set; }

    /// <summary>
    /// Обхват шеи в сантиметрах
    /// </summary>
    public decimal? Neck { get; set; }

    /// <summary>
    /// Обхват плеча в сантиметрах
    /// </summary>
    public decimal? Shoulder { get; set; }

    /// <summary>
    /// Обхват груди в сантиметрах
    /// </summary>
    public decimal? Chest { get; set; }

    /// <summary>
    /// Обхват левого бицепса в сантиметрах
    /// </summary>
    public decimal? LeftBicep { get; set; }

    /// <summary>
    /// Обхват правого бицепса в сантиметрах
    /// </summary>
    public decimal? RightBicep { get; set; }

    /// <summary>
    /// Обхват левого предплечья в сантиметрах
    /// </summary>
    public decimal? LeftForearm { get; set; }

    /// <summary>
    /// Обхват правого предплечья в сантиметрах
    /// </summary>
    public decimal? RightForearm { get; set; }

    /// <summary>
    /// Обхват ягодиц в сантиметрах
    /// </summary>
    public decimal? Buttocks { get; set; }

    /// <summary>
    /// Обхват левого бедра в сантиметрах
    /// </summary>
    public decimal? LeftThigh { get; set; }

    /// <summary>
    /// Обхват правого бедра в сантиметрах
    /// </summary>
    public decimal? RightThigh { get; set; }

    /// <summary>
    /// Обхват левой икры в сантиметрах
    /// </summary>
    public decimal? LeftCalf { get; set; }

    /// <summary>
    /// Обхват правой икры в сантиметрах
    /// </summary>
    public decimal? RightCalf { get; set; }

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

