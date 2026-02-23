using SetRep.Core.Enums;

namespace SetRep.MiniApp.Helpers;

/// <summary>
/// Хелпер для определения времени отдыха по умолчанию в зависимости от группы мышц упражнения
/// </summary>
public static class RestTimerHelper
{
    /// <summary>
    /// Значение по умолчанию, когда группа мышц не задана (в секундах)
    /// </summary>
    public const int DefaultRestSeconds = 60;

    /// <summary>
    /// Получает время отдыха по умолчанию (в секундах) на основе группы мышц упражнения
    /// </summary>
    /// <param name="muscleGroup">Группа мышц упражнения</param>
    /// <returns>Время отдыха в секундах</returns>
    public static int GetDefaultRestSeconds(MuscleGroup? muscleGroup)
    {
        if (!muscleGroup.HasValue)
        {
            return DefaultRestSeconds;
        }

        return muscleGroup.Value switch
        {
            // Большие группы мышц — более длительный отдых (120 сек)
            MuscleGroup.Quadriceps => 120,
            MuscleGroup.Hamstrings => 120,
            MuscleGroup.Glutes => 120,
            MuscleGroup.Back => 120,
            MuscleGroup.Chest => 120,

            // Средние группы мышц — средний отдых (75 сек)
            MuscleGroup.Shoulders => 75,
            MuscleGroup.Traps => 75,
            MuscleGroup.Calves => 75,
            MuscleGroup.Biceps => 75,

            // Малые группы мышц — меньший отдых (60 сек)
            MuscleGroup.Triceps => 60,
            MuscleGroup.Forearms => 60,
            MuscleGroup.Abs => 60,

            // Специальные случаи
            MuscleGroup.Cardio => 60,
            MuscleGroup.Other => 60,

            _ => DefaultRestSeconds
        };
    }
}
