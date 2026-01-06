using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с упражнениями тренировки и подходами
/// </summary>
public class WorkoutExerciseService : IWorkoutExerciseService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public WorkoutExerciseService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
    }

    /// <summary>
    /// Получить упражнение тренировки с подходами
    /// </summary>
    public async Task<WorkoutExercise?> GetWorkoutExerciseWithSetsAsync(Guid workoutExerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workoutexercises/{workoutExerciseId}/with-sets");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutExercise>();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить список подходов для упражнения тренировки
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
    /// Создать новый подход
    /// </summary>
    public async Task<Set?> CreateSetAsync(CreateSetDto createSetDto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/sets")
            {
                Content = JsonContent.Create(createSetDto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Set>();
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

    /// <summary>
    /// Удалить подход
    /// </summary>
    public async Task<bool> DeleteSetAsync(Guid setId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/sets/{setId}");
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
    /// Создать связь тренировки и упражнения
    /// </summary>
    public async Task<WorkoutExercise?> CreateWorkoutExerciseAsync(CreateWorkoutExerciseDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/workoutexercises")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WorkoutExercise>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Обновить связь тренировки и упражнения
    /// </summary>
    public async Task<bool> UpdateWorkoutExerciseAsync(Guid workoutExerciseId, CreateWorkoutExerciseDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/workoutexercises/{workoutExerciseId}")
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
    /// Удалить связь тренировки и упражнения
    /// </summary>
    public async Task<bool> DeleteWorkoutExerciseAsync(Guid workoutExerciseId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workoutexercises/{workoutExerciseId}");
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

