using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Core.Entities;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с историей измерений через API
/// </summary>
public class MeasurementsHistoryService : IMeasurementsHistoryService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;

    public MeasurementsHistoryService(
        HttpClient httpClient,
        ITelegramWebAppStateService telegramWebAppStateService)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
    }

    public async Task<List<MeasurementsHistory>?> GetMeasurementsHistoryByUserIdAsync(Guid userId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/measurementshistory/user/{userId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MeasurementsHistory>>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<MeasurementsHistory?> GetMeasurementsHistoryByIdAsync(Guid id)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/measurementshistory/{id}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementsHistory>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<MeasurementsHistory?> GetLastMeasurementsHistoryAsync(Guid userId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/measurementshistory/user/{userId}/last");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementsHistory>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<MeasurementsHistory?> CreateMeasurementsHistoryAsync(CreateMeasurementsHistoryDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/measurementshistory")
            {
                Content = JsonContent.Create(dto)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MeasurementsHistory>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateMeasurementsHistoryAsync(Guid id, CreateMeasurementsHistoryDto dto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/measurementshistory/{id}")
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

    public async Task<bool> DeleteMeasurementsHistoryAsync(Guid id)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/measurementshistory/{id}");
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
