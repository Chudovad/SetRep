using System.ComponentModel;

namespace SetRep.Core.Enums;

/// <summary>
/// Оборудование для готового плана тренировок
/// </summary>
public enum ReadyPlanEquipment
{
    [Description("Тренажёрный зал")]
    Gym = 1,

    [Description("Гантели")]
    Dumbbells = 2,

    [Description("Нет")]
    None = 3
}
