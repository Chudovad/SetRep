using System.ComponentModel;

namespace SetRep.Core.Enums;

/// <summary>
/// Цель готового плана тренировок
/// </summary>
public enum ReadyPlanGoal
{
    [Description("Нарастить мышцы")]
    BuildMuscle = 1,

    [Description("Увеличить силу")]
    IncreaseStrength = 2,

    [Description("Сбросить вес")]
    LoseWeight = 3
}
