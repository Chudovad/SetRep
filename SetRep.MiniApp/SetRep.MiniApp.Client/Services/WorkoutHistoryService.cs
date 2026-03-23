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

    public async Task<bool> EndWorkoutAsync(Guid workoutHistoryId, bool updateWorkoutTemplateFromHistory = true)
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

            if (updateWorkoutTemplateFromHistory)
            {
                await UpdateWorkoutSetsFromHistoryAsync(workoutHistoryId, existing.WorkoutId);
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

    /// <summary>
    /// Синхронизирует подходы шаблона тренировки с выполненными подходами из истории.
    /// Обновляет существующие подходы и создаёт в шаблоне недостающие номера подходов из истории.
    /// Подставляет вес и повторения из истории (в том числе меньшие, чем в шаблоне).
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

                var workoutSets = await _setService.GetSetsByWorkoutExerciseIdAsync(workoutExercise.Id)
                                   ?? new List<Set>();
                var existingSetNumbers = workoutSets.Select(s => s.SetNumber).ToHashSet();

                var historyValuesBySetNumber = exerciseHistorySets
                    .GroupBy(s => s.SetNumber)
                    .ToDictionary(
                        g => g.Key,
                        g =>
                        {
                            var withWeight = g.Where(s => s.Weight.HasValue).ToList();
                            var latest = g.OrderByDescending(s => s.CompletedAt).First();
                            return (
                                HistWeight: withWeight.Count > 0 ? withWeight.Max(s => s.Weight!.Value) : (decimal?)null,
                                HistReps: g.Max(s => s.Reps),
                                DurationSeconds: latest.DurationSeconds,
                                DistanceMeters: latest.DistanceMeters,
                                Notes: latest.Notes,
                                CompletedAt: latest.CompletedAt
                            );
                        });

                foreach (var workoutSet in workoutSets)
                {
                    if (!historyValuesBySetNumber.TryGetValue(workoutSet.SetNumber, out var historyValues))
                        continue;

                    var newWeight = historyValues.HistWeight ?? workoutSet.Weight;
                    var newReps = historyValues.HistReps;

                    var weightDiffers = historyValues.HistWeight.HasValue
                        ? !workoutSet.Weight.HasValue || workoutSet.Weight.Value != historyValues.HistWeight.Value
                        : false;
                    if (weightDiffers || newReps != workoutSet.Reps)
                    {
                        var updateDto = new CreateSetDto
                        {
                            WorkoutExerciseId = workoutSet.WorkoutExerciseId,
                            SetNumber = workoutSet.SetNumber,
                            Reps = newReps,
                            Weight = newWeight,
                            DurationSeconds = workoutSet.DurationSeconds,
                            DistanceMeters = workoutSet.DistanceMeters,
                            IsCompleted = workoutSet.IsCompleted,
                            Notes = workoutSet.Notes,
                            CompletedAt = workoutSet.CompletedAt
                        };
                        await _setService.UpdateSetAsync(workoutSet.Id, updateDto);
                    }
                }

                foreach (var (setNumber, hv) in historyValuesBySetNumber)
                {
                    if (existingSetNumbers.Contains(setNumber))
                        continue;

                    var completedAt = hv.CompletedAt == default ? DateTime.UtcNow : hv.CompletedAt;
                    var createDto = new CreateSetDto
                    {
                        WorkoutExerciseId = workoutExercise.Id,
                        SetNumber = setNumber,
                        Reps = hv.HistReps,
                        Weight = hv.HistWeight,
                        DurationSeconds = hv.DurationSeconds,
                        DistanceMeters = hv.DistanceMeters,
                        IsCompleted = true,
                        Notes = hv.Notes,
                        CompletedAt = completedAt
                    };
                    await _setService.CreateSetAsync(createDto);
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
