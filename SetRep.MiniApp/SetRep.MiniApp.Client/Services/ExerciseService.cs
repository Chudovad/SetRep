using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с упражнениями
/// </summary>
public class ExerciseService : IExerciseService
{
    private readonly HttpClient _httpClient;

    public ExerciseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить все упражнения
    /// </summary>
    public async Task<List<Exercise>?> GetExercisesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/exercises");
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
            var response = await _httpClient.GetAsync($"api/exercises/{exerciseId}");
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
            var response = await _httpClient.PostAsJsonAsync("api/exercises", exercise);
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
}

