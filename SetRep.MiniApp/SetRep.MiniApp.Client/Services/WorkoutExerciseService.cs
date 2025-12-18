using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с упражнениями тренировки и подходами
/// </summary>
public class WorkoutExerciseService : IWorkoutExerciseService
{
    private readonly HttpClient _httpClient;

    public WorkoutExerciseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить упражнение тренировки с подходами
    /// </summary>
    public async Task<WorkoutExercise?> GetWorkoutExerciseWithSetsAsync(Guid workoutExerciseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/workoutexercises/{workoutExerciseId}/with-sets");

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
            var response = await _httpClient.GetAsync($"api/sets/workout-exercise/{workoutExerciseId}");

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
            var response = await _httpClient.PostAsJsonAsync("api/sets", createSetDto);

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
            var response = await _httpClient.PutAsJsonAsync($"api/sets/{setId}", dto);
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
            var response = await _httpClient.DeleteAsync($"api/sets/{setId}");
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
            var response = await _httpClient.PostAsJsonAsync("api/workoutexercises", dto);
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
}

