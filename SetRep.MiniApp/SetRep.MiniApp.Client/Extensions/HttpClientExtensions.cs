using SetRep.MiniApp.Client.Services;
using System.Net.Http.Headers;

namespace SetRep.MiniApp.Client.Extensions;

/// <summary>
/// Расширения для HttpClient для добавления JWT токена
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Добавляет JWT токен в заголовок Authorization запроса
    /// </summary>
    public static void AddBearerToken(this HttpRequestMessage request, ITelegramWebAppStateService telegramWebAppStateService)
    {
        var token = telegramWebAppStateService.ValidationResult?.Data?.Token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

