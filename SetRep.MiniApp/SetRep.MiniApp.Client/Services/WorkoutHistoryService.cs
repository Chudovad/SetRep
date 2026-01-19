using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с историей тренировок через API
/// </summary>
public class WorkoutHistoryService : IWorkoutHistoryService
{
    private readonly HttpClient _httpClient;
    private readonly IActiveWorkoutService _activeWorkoutService;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public WorkoutHistoryService(HttpClient httpClient, IActiveWorkoutService activeWorkoutService, ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _activeWorkoutService = activeWorkoutService;
        _telegramWebAppStateService = telegramWebAppStateService;
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

            var request = new HttpRequestMessage(HttpMethod.Post, "api/workouthistory")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
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
            var getRequest = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistory/{workoutHistoryId}");
            getRequest.AddBearerToken(_telegramWebAppStateService);
            var getResponse = await _httpClient.SendAsync(getRequest);
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
                WorkoutId = existing.WorkoutId.Value,
                UserId = existing.UserId,
                StartTime = existing.StartTime,
                EndTime = DateTime.UtcNow,
                Notes = existing.Notes
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"api/workouthistory/{workoutHistoryId}")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistory/user/{userId}/last");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
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
            var workoutRequest = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/{workoutId}/details");
            workoutRequest.AddBearerToken(_telegramWebAppStateService);
            var workoutResponse = await _httpClient.SendAsync(workoutRequest);
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

                    var request = new HttpRequestMessage(HttpMethod.Post, "api/workouthistorysets")
                    {
                        Content = JsonContent.Create(historySetDto)
                    };
                    request.AddBearerToken(_telegramWebAppStateService);
                    await _httpClient.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistory/user/{userId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistory/{workoutHistoryId}/details");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
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

    public async Task<bool> DeleteWorkoutHistoryAsync(Guid workoutHistoryId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workouthistory/{workoutHistoryId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

}
