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
    /// Получить все тренировки для конкретного пользователя
    /// </summary>
    public async Task<List<Workout>?> GetWorkoutsAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workouts?userId={userId}");
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

    /// <summary>
    /// Удалить тренировку
    /// </summary>
    public async Task<bool> DeleteWorkoutAsync(Guid workoutId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/workouts/{workoutId}");
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
            var response = await _httpClient.PutAsJsonAsync($"api/workouts/{workoutId}", dto);
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
            var getResponse = await _httpClient.GetAsync($"api/workoutexercises/{workoutExerciseId}");
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

            var response = await _httpClient.PutAsJsonAsync($"api/workoutexercises/{workoutExerciseId}", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

