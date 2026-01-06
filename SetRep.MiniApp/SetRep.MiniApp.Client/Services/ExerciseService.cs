using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с упражнениями
/// </summary>
public class ExerciseService : IExerciseService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;
    private readonly ILogger<ExerciseService>? _logger;

    public ExerciseService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService, ILogger<ExerciseService>? logger = null)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
        _logger = logger;
    }

    /// <summary>
    /// Получить все упражнения
    /// </summary>
    public async Task<List<Exercise>?> GetExercisesAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/exercises");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Exercise>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить упражнение по идентификатору
    /// </summary>
    public async Task<Exercise?> GetExerciseByIdAsync(Guid exerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/exercises/{exerciseId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Exercise>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Создать новое упражнение
    /// </summary>
    public async Task<Exercise?> CreateExerciseAsync(Exercise exercise)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/exercises")
            {
                Content = JsonContent.Create(exercise)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Exercise>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Обновить упражнение
    /// </summary>
    public async Task<bool> UpdateExerciseAsync(Exercise exercise)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/exercises/{exercise.Id}")
            {
                Content = JsonContent.Create(exercise)
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
    /// Удалить упражнение
    /// </summary>
    public async Task<bool> DeleteExerciseAsync(Guid exerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/exercises/{exerciseId}");
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

