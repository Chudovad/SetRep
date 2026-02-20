using System.ComponentModel;

namespace SetRep.Core.Enums;

/// <summary>
/// Уровень подготовки для готового плана тренировок
/// </summary>
public enum ReadyPlanLevel
{
    [Description("Новичок")]
    Beginner = 1,

    [Description("Средний")]
    Intermediate = 2,

    [Description("Продвинутый")]
    Advanced = 3
}
