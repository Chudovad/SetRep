using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.Shared.DTO;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;
    private readonly ILogger<UserService>? _logger;

    public UserService(HttpClient httpClient, ITelegramWebAppStateService telegramWebAppStateService, ILogger<UserService>? logger = null)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
        _logger = logger;
    }

    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    public async Task<UserDto?> GetUserByTelegramIdAsync(long telegramId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Users/telegram/{telegramId}");
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    public async Task<UserDto?> CreateUserAsync(UserDto user)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Users")
            {
                Content = JsonContent.Create(user)
            };
            request.AddBearerToken(_telegramWebAppStateService);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}

