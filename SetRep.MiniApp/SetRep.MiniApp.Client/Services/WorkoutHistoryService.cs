using System.Net.Http.Json;
using System.Timers;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с историей тренировок
/// </summary>
public class WorkoutHistoryService : IWorkoutHistoryService, IDisposable
{
    private readonly HttpClient _httpClient;
    private System.Timers.Timer? _timer;
    private WorkoutHistory? _activeWorkout;
    private DateTime _startTime;

    public WorkoutHistoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public event Action<TimeSpan>? DurationUpdated;

    public async Task<WorkoutHistory?> StartWorkoutAsync(Guid workoutId, Guid userId)
    {
        try
        {
            var dto = new CreateWorkoutHistoryDto
            {
                WorkoutId = workoutId,
                UserId = userId,
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            var response = await _httpClient.PostAsJsonAsync("api/workouthistory", dto);
            
            if (response.IsSuccessStatusCode)
            {
                _activeWorkout = await response.Content.ReadFromJsonAsync<WorkoutHistory>();
                if (_activeWorkout != null)
                {
                    _startTime = _activeWorkout.StartTime;
                    StartTimer();
                }
                return _activeWorkout;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> EndWorkoutAsync(Guid workoutHistoryId)
    {
        try
        {
            StopTimer();
            
            // Получаем текущую запись
            var getResponse = await _httpClient.GetAsync($"api/workouthistory/{workoutHistoryId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var existing = await getResponse.Content.ReadFromJsonAsync<WorkoutHistory>();
            if (existing == null)
            {
                return false;
            }

            // Обновляем только EndTime
            var dto = new CreateWorkoutHistoryDto
            {
                WorkoutId = existing.WorkoutId,
                UserId = existing.UserId,
                StartTime = existing.StartTime,
                EndTime = DateTime.UtcNow,
                Notes = existing.Notes
            };

            var response = await _httpClient.PutAsJsonAsync($"api/workouthistory/{workoutHistoryId}", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<WorkoutHistory?> GetActiveWorkoutAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workouthistory/user/{userId}/last");
            
            if (response.IsSuccessStatusCode)
            {
                var workoutHistory = await response.Content.ReadFromJsonAsync<WorkoutHistory>();
                if (workoutHistory != null && workoutHistory.EndTime == null)
                {
                    _activeWorkout = workoutHistory;
                    _startTime = workoutHistory.StartTime;
                    StartTimer();
                    return workoutHistory;
                }
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CopySetsFromWorkoutAsync(Guid workoutHistoryId, Guid workoutId)
    {
        try
        {
            // Получаем тренировку с упражнениями и подходами
            var workoutResponse = await _httpClient.GetAsync($"api/workouts/{workoutId}/details");
            if (!workoutResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var workout = await workoutResponse.Content.ReadFromJsonAsync<Workout>();
            if (workout?.WorkoutExercises == null)
            {
                return false;
            }

            // Копируем подходы для каждого упражнения
            foreach (var workoutExercise in workout.WorkoutExercises.OrderBy(we => we.Order))
            {
                if (workoutExercise.Sets == null || !workoutExercise.Sets.Any())
                {
                    continue;
                }

                foreach (var set in workoutExercise.Sets.OrderBy(s => s.SetNumber))
                {
                    var historySetDto = new CreateWorkoutHistorySetDto
                    {
                        WorkoutHistoryId = workoutHistoryId,
                        ExerciseId = workoutExercise.ExerciseId,
                        SetNumber = set.SetNumber,
                        Reps = set.Reps,
                        Weight = set.Weight,
                        DurationSeconds = set.DurationSeconds,
                        DistanceMeters = set.DistanceMeters,
                        RestSeconds = set.RestSeconds,
                        IsCompleted = false,
                        Notes = set.Notes,
                        CompletedAt = DateTime.UtcNow
                    };

                    await _httpClient.PostAsJsonAsync("api/workouthistorysets", historySetDto);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<WorkoutHistory>?> GetWorkoutHistoryByUserIdAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workouthistory/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<WorkoutHistory>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<WorkoutHistory?> GetWorkoutHistoryDetailsAsync(Guid workoutHistoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workouthistory/{workoutHistoryId}/details");
            if (response.IsSuccessStatusCode)
            {
                var workoutHistory = await response.Content.ReadFromJsonAsync<WorkoutHistory>();
                if (workoutHistory != null && workoutHistory.EndTime == null)
                {
                    _activeWorkout = workoutHistory;
                    _startTime = workoutHistory.StartTime;
                    StartTimer();
                }
                return workoutHistory;
            }
            return null;
        }
        catch
        {
            return null;
        }
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
