using System.Timers;
using SetRep.Core.Entities;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для управления активной тренировкой (таймер, события)
/// </summary>
public class ActiveWorkoutService : IActiveWorkoutService
{
    private System.Timers.Timer? _timer;
    private WorkoutHistory? _activeWorkout;
    private DateTime _startTime;

    public event Action<TimeSpan>? DurationUpdated;

    public void SetActiveWorkout(WorkoutHistory workoutHistory)
    {
        _activeWorkout = workoutHistory;
        _startTime = workoutHistory.StartTime;
        StartTimer();
    }

    public void StopTracking()
    {
        StopTimer();
        _activeWorkout = null;
    }

    public TimeSpan GetWorkoutDuration(WorkoutHistory workoutHistory)
    {
        if (workoutHistory.EndTime.HasValue)
        {
            return workoutHistory.EndTime.Value - workoutHistory.StartTime;
        }
        
        return DateTime.UtcNow - workoutHistory.StartTime;
    }

    private void StartTimer()
    {
        StopTimer();
        
        _timer = new System.Timers.Timer(1000); // Обновление каждую секунду
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_activeWorkout != null)
        {
            var duration = GetWorkoutDuration(_activeWorkout);
            DurationUpdated?.Invoke(duration);
        }
    }

    public void Dispose()
    {
        StopTimer();
    }
}
