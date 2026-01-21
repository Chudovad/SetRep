using SetRep.Core.Attributes;
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
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%93%D1%80%D1%83%D0%B4%D1%8C.png")]
    Chest = 1,

    /// <summary>
    /// Спина
    /// </summary>
    [Description("Спина")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%A1%D0%BF%D0%B8%D0%BD%D0%B0.png")]
    Back = 2,

    /// <summary>
    /// Плечи
    /// </summary>
    [Description("Плечи")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%9F%D0%BB%D0%B5%D1%87%D0%B8.png")]
    Shoulders = 3,

    /// <summary>
    /// Бицепс
    /// </summary>
    [Description("Бицепс")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%91%D0%B8%D1%86%D0%B5%D0%BF%D1%81.png")]
    Biceps = 4,

    /// <summary>
    /// Трицепс
    /// </summary>
    [Description("Трицепс")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%A2%D1%80%D0%B8%D1%86%D0%B5%D0%BF%D1%81.png")]
    Triceps = 5,

    /// <summary>
    /// Ноги (квадрицепсы)
    /// </summary>
    [Description("Квадрицепсы")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%9A%D0%B2%D0%B0%D0%B4%D1%80%D0%B8%D1%86%D0%B5%D0%BF%D1%81%D1%8B.png")]
    Quadriceps = 6,

    /// <summary>
    /// Бицепс бедра
    /// </summary>
    [Description("Бицепс бедра")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%91%D0%B8%D1%86%D0%B5%D0%BF%D1%81%20%D0%B1%D0%B5%D0%B4%D1%80%D0%B0.png")]
    Hamstrings = 7,

    /// <summary>
    /// Ягодицы
    /// </summary>
    [Description("Ягодицы")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%AF%D0%B3%D0%BE%D0%B4%D0%B8%D1%86%D1%8B.png")]
    Glutes = 8,

    /// <summary>
    /// Икры
    /// </summary>
    [Description("Икры")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%98%D0%BA%D1%80%D1%8B.png")]
    Calves = 9,

    /// <summary>
    /// Пресс
    /// </summary>
    [Description("Пресс")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%9F%D1%80%D0%B5%D1%81%D1%81.png")]
    Abs = 10,

    /// <summary>
    /// Предплечья
    /// </summary>
    [Description("Предплечья")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%9F%D1%80%D0%B5%D0%B4%D0%BF%D0%BB%D0%B5%D1%87%D1%8C%D1%8F.png")]
    Forearms = 11,

    /// <summary>
    /// Трапеции
    /// </summary>
    [Description("Трапеции")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%A2%D1%80%D0%B0%D0%BF%D0%B5%D1%86%D0%B8%D0%B8.png")]
    Traps = 12,

    /// <summary>
    /// Кардио
    /// </summary>
    [Description("Кардио")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%9A%D0%B0%D1%80%D0%B4%D0%B8%D0%BE.png")]
    Cardio = 13,

    /// <summary>
    /// Другое
    /// </summary>
    [Description("Другое")]
    [ImageUrl("https://storage.yandexcloud.net/img/SetRep/MuscleGroup/%D0%94%D1%80%D1%83%D0%B3%D0%BE%D0%B5.png")]
    Other = 15
}

