using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с подходами тренировки
/// </summary>
public class SetService : ISetService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public SetService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
    }

    /// <summary>
    /// Получить подходы упражнения в тренировке
    /// </summary>
    public async Task<List<Set>?> GetSetsByWorkoutExerciseIdAsync(Guid workoutExerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/sets/workout-exercise/{workoutExerciseId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Set>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить подходы тренировки
    /// </summary>
    public async Task<List<Set>?> GetSetsByWorkoutIdAsync(Guid workoutId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/sets/workout/{workoutId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Set>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Обновить подход
    /// </summary>
    public async Task<bool> UpdateSetAsync(Guid setId, CreateSetDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/sets/{setId}")
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
}
