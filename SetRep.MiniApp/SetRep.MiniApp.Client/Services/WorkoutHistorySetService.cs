using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с подходами в истории тренировок
/// </summary>
public class WorkoutHistorySetService : IWorkoutHistorySetService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public WorkoutHistorySetService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
    }

    /// <summary>
    /// Получить подходы для упражнения в истории тренировки
    /// </summary>
    public async Task<List<WorkoutHistorySet>?> GetSetsByWorkoutHistoryAndExerciseAsync(Guid workoutHistoryId, Guid exerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistorysets/workout-history/{workoutHistoryId}/exercise/{exerciseId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<WorkoutHistorySet>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Создать новый подход в истории тренировки
    /// </summary>
    public async Task<WorkoutHistorySet?> CreateSetAsync(CreateWorkoutHistorySetDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/workouthistorysets")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutHistorySet>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Обновить подход в истории тренировки
    /// </summary>
    public async Task<bool> UpdateSetAsync(Guid setId, CreateWorkoutHistorySetDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/workouthistorysets/{setId}")
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
    /// Обновить статус выполнения подхода
    /// </summary>
    public async Task<bool> ToggleSetCompletionAsync(Guid setId, bool isCompleted)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/workouthistorysets/{setId}/complete")
            {
                Content = JsonContent.Create(isCompleted)
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
    /// Удалить подход из истории тренировки
    /// </summary>
    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workouthistorysets/{setId}");
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

