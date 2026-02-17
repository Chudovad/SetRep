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
    private readonly IWorkoutHistorySetService _workoutHistorySetService;
    private readonly IWorkoutService _workoutService;
    private readonly ISetService _setService;

    public WorkoutHistoryService(
        HttpClient httpClient,
        IActiveWorkoutService activeWorkoutService,
        ITelegramWebAppStateService telegramWebAppStateService,
        IWorkoutHistorySetService workoutHistorySetService,
        IWorkoutService workoutService,
        ISetService setService)
    {
        _httpClient = httpClient;
        _activeWorkoutService = activeWorkoutService;
        _telegramWebAppStateService = telegramWebAppStateService;
        _workoutHistorySetService = workoutHistorySetService;
        _workoutService = workoutService;
        _setService = setService;
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

            await UpdateWorkoutSetsFromHistoryAsync(workoutHistoryId, existing.WorkoutId);

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

    /// <summary>
    /// Синхронизирует подходы шаблона тренировки с выполненными подходами из истории.
    /// Обновляет вес и повторения в шаблоне, если в истории есть лучшие результаты.
    /// </summary>
    private async Task UpdateWorkoutSetsFromHistoryAsync(Guid workoutHistoryId, Guid? workoutId)
    {
        if (!workoutId.HasValue)
            return;

        try
        {
            var historySets = await _workoutHistorySetService.GetSetsByWorkoutHistoryIdAsync(workoutHistoryId);
            if (historySets == null || !historySets.Any())
                return;

            var workoutDetails = await _workoutService.GetWorkoutDetailsAsync(workoutId.Value);
            if (workoutDetails?.WorkoutExercises == null)
                return;

            var historySetsByExercise = historySets
                .Where(s => s.IsCompleted)
                .GroupBy(s => s.ExerciseId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var workoutExercise in workoutDetails.WorkoutExercises)
            {
                if (!historySetsByExercise.TryGetValue(workoutExercise.ExerciseId, out var exerciseHistorySets))
                    continue;

                var workoutSets = await _setService.GetSetsByWorkoutExerciseIdAsync(workoutExercise.Id);
                if (workoutSets == null || !workoutSets.Any())
                    continue;

                var maxValuesBySetNumber = exerciseHistorySets
                    .GroupBy(s => s.SetNumber)
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            MaxWeight = g.Where(s => s.Weight.HasValue).Max(s => s.Weight),
                            MaxReps = g.Max(s => s.Reps)
                        });

                foreach (var workoutSet in workoutSets)
                {
                    if (!maxValuesBySetNumber.TryGetValue(workoutSet.SetNumber, out var maxValues))
                        continue;

                    bool needsUpdate = false;
                    decimal? newWeight = workoutSet.Weight;
                    int newReps = workoutSet.Reps;

                    if (maxValues.MaxWeight.HasValue)
                    {
                        if (!workoutSet.Weight.HasValue || maxValues.MaxWeight.Value > workoutSet.Weight.Value)
                        {
                            newWeight = maxValues.MaxWeight.Value;
                            needsUpdate = true;
                        }
                    }

                    if (maxValues.MaxReps > workoutSet.Reps)
                    {
                        newReps = maxValues.MaxReps;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        var updateDto = new CreateSetDto
                        {
                            WorkoutExerciseId = workoutSet.WorkoutExerciseId,
                            SetNumber = workoutSet.SetNumber,
                            Reps = newReps,
                            Weight = newWeight,
                            DurationSeconds = workoutSet.DurationSeconds,
                            DistanceMeters = workoutSet.DistanceMeters,
                            RestSeconds = workoutSet.RestSeconds,
                            IsCompleted = workoutSet.IsCompleted,
                            Notes = workoutSet.Notes,
                            CompletedAt = workoutSet.CompletedAt
                        };
                        await _setService.UpdateSetAsync(workoutSet.Id, updateDto);
                    }
                }
            }
        }
        catch
        {
            // Не прерываем завершение тренировки при ошибке обновления подходов
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
