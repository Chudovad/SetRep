using System.Net.Http.Json;
using SetRep.Shared.DTO;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    public async Task<UserDto?> GetUserByTelegramIdAsync(long telegramId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Users/telegram/{telegramId}");
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

