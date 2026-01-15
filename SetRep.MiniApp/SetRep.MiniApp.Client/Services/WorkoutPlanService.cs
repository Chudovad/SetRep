using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с планами тренировок
/// </summary>
public class WorkoutPlanService : IWorkoutPlanService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;
    private readonly ILogger<WorkoutPlanService>? _logger;

    public WorkoutPlanService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService, ILogger<WorkoutPlanService>? logger = null)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
        _logger = logger;
    }

    /// <summary>
    /// Получить все планы тренировок для конкретного пользователя
    /// </summary>
    public async Task<List<WorkoutPlan>?> GetWorkoutPlansAsync(Guid userId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workoutplans?userId={userId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<WorkoutPlan>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить план тренировки по идентификатору
    /// </summary>
    public async Task<WorkoutPlan?> GetWorkoutPlanByIdAsync(Guid workoutPlanId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workoutplans/{workoutPlanId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutPlan>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить план тренировки с деталями (тренировками)
    /// </summary>
    public async Task<WorkoutPlan?> GetWorkoutPlanDetailsAsync(Guid workoutPlanId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workoutplans/{workoutPlanId}/details");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutPlan>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Создать новый план тренировки
    /// </summary>
    public async Task<WorkoutPlan?> CreateWorkoutPlanAsync(CreateWorkoutPlanDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/workoutplans")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutPlan>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Обновить план тренировки
    /// </summary>
    public async Task<bool> UpdateWorkoutPlanAsync(Guid workoutPlanId, CreateWorkoutPlanDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/workoutplans/{workoutPlanId}")
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
    /// Удалить план тренировки
    /// </summary>
    public async Task<bool> DeleteWorkoutPlanAsync(Guid workoutPlanId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workoutplans/{workoutPlanId}");
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
