using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с подходами в истории тренировок
/// </summary>
public class WorkoutHistorySetService : IWorkoutHistorySetService
{
    private readonly HttpClient _httpClient;

    public WorkoutHistorySetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить подходы для упражнения в истории тренировки
    /// </summary>
    public async Task<List<WorkoutHistorySet>?> GetSetsByWorkoutHistoryAndExerciseAsync(Guid workoutHistoryId, Guid exerciseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workouthistorysets/workout-history/{workoutHistoryId}/exercise/{exerciseId}");
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
            var response = await _httpClient.PostAsJsonAsync("api/workouthistorysets", dto);
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
            var response = await _httpClient.PutAsJsonAsync($"api/workouthistorysets/{setId}", dto);
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
            var response = await _httpClient.PatchAsJsonAsync($"api/workouthistorysets/{setId}/complete", isCompleted);
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
            var response = await _httpClient.DeleteAsync($"api/workouthistorysets/{setId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

