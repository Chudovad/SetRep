using SetRep.Core.Enums;

namespace SetRep.MiniApp.Client.Helpers;

/// <summary>
/// Хелпер для определения шага изменения веса в зависимости от группы мышц упражнения
/// </summary>
public static class ExerciseStepHelper
{
    /// <summary>
    /// Получает шаг для MudNumericField на основе группы мышц упражнения
    /// </summary>
    /// <param name="muscleGroup">Группа мышц упражнения</param>
    /// <returns>Шаг для изменения веса</returns>
    public static decimal GetWeightStep(MuscleGroup? muscleGroup)
    {
        if (!muscleGroup.HasValue)
        {
            return 0.5m;
        }

        return muscleGroup.Value switch
        {
            // Большие группы мышц - больший шаг (2.5 кг)
            MuscleGroup.Quadriceps => 2.5m,
            MuscleGroup.Hamstrings => 2.5m,
            MuscleGroup.Glutes => 2.5m,
            MuscleGroup.Back => 2.5m,
            MuscleGroup.Chest => 2.5m,
            MuscleGroup.Calves => 2.5m,
            
            // Средние группы мышц - средний шаг (1 кг)
            MuscleGroup.Shoulders => 1.0m,
            MuscleGroup.Traps => 1.0m,
            
            // Малые группы мышц - меньший шаг (0.5 кг)
            MuscleGroup.Biceps => 0.5m,
            MuscleGroup.Triceps => 0.5m,
            MuscleGroup.Forearms => 0.5m,
            MuscleGroup.Abs => 0.5m,
            
            // Специальные случаи
            MuscleGroup.Cardio => 0.5m,
            MuscleGroup.Other => 0.5m,
            
            _ => 0.5m
        };
    }
}

