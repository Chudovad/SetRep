using System.ComponentModel;

namespace SetRep.Core.Enums;

/// <summary>
/// Группы мышц для классификации упражнений
/// </summary>
public enum MuscleGroup
{
    /// <summary>
    /// Грудь
    /// </summary>
    [Description("Грудь")]
    Chest = 1,

    /// <summary>
    /// Спина
    /// </summary>
    [Description("Спина")]
    Back = 2,

    /// <summary>
    /// Плечи
    /// </summary>
    [Description("Плечи")]
    Shoulders = 3,

    /// <summary>
    /// Бицепс
    /// </summary>
    [Description("Бицепс")]
    Biceps = 4,

    /// <summary>
    /// Трицепс
    /// </summary>
    [Description("Трицепс")]
    Triceps = 5,

    /// <summary>
    /// Ноги (квадрицепсы)
    /// </summary>
    [Description("Ноги")]
    Quadriceps = 6,

    /// <summary>
    /// Бицепс бедра
    /// </summary>
    [Description("Бицепс бедра")]
    Hamstrings = 7,

    /// <summary>
    /// Ягодицы
    /// </summary>
    [Description("Ягодицы")]
    Glutes = 8,

    /// <summary>
    /// Икры
    /// </summary>
    [Description("Икры")]
    Calves = 9,

    /// <summary>
    /// Пресс
    /// </summary>
    [Description("Пресс")]
    Abs = 10,

    /// <summary>
    /// Предплечья
    /// </summary>
    [Description("Предплечья")]
    Forearms = 11,

    /// <summary>
    /// Трапеции
    /// </summary>
    [Description("Трапеции")]
    Traps = 12,

    /// <summary>
    /// Кардио
    /// </summary>
    [Description("Кардио")]
    Cardio = 13,

    /// <summary>
    /// Полное тело
    /// </summary>
    [Description("Полное тело")]
    FullBody = 14,

    /// <summary>
    /// Другое
    /// </summary>
    [Description("Другое")]
    Other = 15
}

