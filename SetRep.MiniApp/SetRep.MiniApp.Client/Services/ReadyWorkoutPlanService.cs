using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с готовыми планами тренировок
/// </summary>
public class ReadyWorkoutPlanService : IReadyWorkoutPlanService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;
    private readonly ILogger<ReadyWorkoutPlanService>? _logger;

    public ReadyWorkoutPlanService(
        HttpClient httpClient,
        ITelegramWebAppStateService telegramWebAppStateService,
        ILogger<ReadyWorkoutPlanService>? logger = null)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
        _logger = logger;
    }

    public async Task<List<ReadyWorkoutPlan>?> GetReadyPlansAsync(
        ReadyPlanLevel? level = null,
        ReadyPlanGoal? goal = null,
        ReadyPlanEquipment? equipment = null)
    {
        try
        {
            var query = new List<string>();
            if (level.HasValue) query.Add($"level={(int)level.Value}");
            if (goal.HasValue) query.Add($"goal={(int)goal.Value}");
            if (equipment.HasValue) query.Add($"equipment={(int)equipment.Value}");
            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/readyworkoutplans{queryString}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<ReadyWorkoutPlan>>();
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReadyWorkoutPlan?> GetReadyPlanByWorkoutPlanIdAsync(Guid planId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/readyworkoutplans/by-plan/{planId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ReadyWorkoutPlan>();
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReadyWorkoutPlan?> AddReadyPlanAsync(CreateReadyWorkoutPlanDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/readyworkoutplans")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ReadyWorkoutPlan>();
            return null;
        }
        catch
        {
            return null;
        }
    }
}
