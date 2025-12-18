using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с историей тренировок через API
/// </summary>
public class WorkoutHistoryService : IWorkoutHistoryService
{
    private readonly HttpClient _httpClient;
    private readonly IActiveWorkoutService _activeWorkoutService;

    public WorkoutHistoryService(HttpClient httpClient, IActiveWorkoutService activeWorkoutService)
    {
        _httpClient = httpClient;
        _activeWorkoutService = activeWorkoutService;
    }

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
                var workoutHistory = await response.Content.ReadFromJsonAsync<WorkoutHistory>();
                if (workoutHistory != null)
                {
                    _activeWorkoutService.SetActiveWorkout(workoutHistory);
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

    public async Task<bool> EndWorkoutAsync(Guid workoutHistoryId)
    {
        try
        {
            _activeWorkoutService.StopTracking();
            
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
                    _activeWorkoutService.SetActiveWorkout(workoutHistory);
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
                    _activeWorkoutService.SetActiveWorkout(workoutHistory);
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

}
