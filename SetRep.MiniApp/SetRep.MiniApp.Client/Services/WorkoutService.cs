using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с тренировками
/// </summary>
public class WorkoutService : IWorkoutService
{
    private readonly HttpClient _httpClient;

    public WorkoutService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить все тренировки
    /// </summary>
    public async Task<List<Workout>?> GetWorkoutsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/workouts");
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
            var response = await _httpClient.GetAsync($"api/workouts/{workoutId}/details");
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
            var response = await _httpClient.PostAsJsonAsync("api/workouts", dto);
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
            var response = await _httpClient.GetAsync($"api/workouthistory/workout/{workoutId}");
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
}

