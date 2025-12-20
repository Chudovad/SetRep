using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с тренировками
/// </summary>
public class WorkoutService : IWorkoutService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public WorkoutService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
    }

    /// <summary>
    /// Получить все тренировки для конкретного пользователя
    /// </summary>
    public async Task<List<Workout>?> GetWorkoutsAsync(Guid userId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts?userId={userId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Workout>>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить детали тренировки с упражнениями
    /// </summary>
    public async Task<Workout?> GetWorkoutDetailsAsync(Guid workoutId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/{workoutId}/details");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Workout>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Создать новую тренировку
    /// </summary>
    public async Task<Workout?> CreateWorkoutAsync(CreateWorkoutDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/workouts")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Workout>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить историю тренировок для шаблона тренировки
    /// </summary>
    public async Task<List<WorkoutHistory>?> GetWorkoutHistoryByWorkoutIdAsync(Guid workoutId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouthistory/workout/{workoutId}");
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

    /// <summary>
    /// Удалить тренировку
    /// </summary>
    public async Task<bool> DeleteWorkoutAsync(Guid workoutId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workouts/{workoutId}");
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
    /// Обновить тренировку
    /// </summary>
    public async Task<bool> UpdateWorkoutAsync(Guid workoutId, CreateWorkoutDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/workouts/{workoutId}")
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
    /// Обновить порядок упражнения в тренировке
    /// </summary>
    public async Task<bool> UpdateWorkoutExerciseOrderAsync(Guid workoutExerciseId, int newOrder)
    {
        try
        {
            // Сначала получаем текущее упражнение
            var getRequest = new HttpRequestMessage(HttpMethod.Get, $"api/workoutexercises/{workoutExerciseId}");
            getRequest.AddBearerToken(_telegramWebAppStateService);
            var getResponse = await _httpClient.SendAsync(getRequest);
            if (!getResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var workoutExercise = await getResponse.Content.ReadFromJsonAsync<WorkoutExercise>();
            if (workoutExercise == null)
            {
                return false;
            }

            // Обновляем только Order
            var dto = new CreateWorkoutExerciseDto
            {
                WorkoutId = workoutExercise.WorkoutId,
                ExerciseId = workoutExercise.ExerciseId,
                Order = newOrder,
                Notes = workoutExercise.Notes
            };

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
}

