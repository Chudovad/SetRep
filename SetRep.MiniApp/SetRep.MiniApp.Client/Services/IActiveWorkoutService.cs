using SetRep.Core.Entities;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для управления активной тренировкой (таймер, события)
/// </summary>
public interface IActiveWorkoutService : IDisposable
{
    /// <summary>
    /// Установить активную тренировку и начать отслеживание времени
    /// </summary>
    void SetActiveWorkout(WorkoutHistory workoutHistory);

    /// <summary>
    /// Остановить отслеживание активной тренировки
    /// </summary>
    void StopTracking();

    /// <summary>
    /// Получить текущую длительность активной тренировки
    /// </summary>
    TimeSpan GetWorkoutDuration(WorkoutHistory workoutHistory);

    /// <summary>
    /// Событие обновления времени тренировки
    /// </summary>
    event Action<TimeSpan>? DurationUpdated;
}
