using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using SetRep.Shared.Telegram;

namespace SetRep.WebApp.Client.Services;

public class TelegramWebAppService : ITelegramWebAppService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public TelegramWebAppService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public async Task<TelegramWebAppValidationResult?> ValidateAsync(string tgWebAppData)
    {
        try
        {
            var request = new { TgWebAppData = tgWebAppData };
            var response = await _httpClient.PostAsJsonAsync("api/telegramwebapp/validate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TelegramWebAppValidationResponse>();
                if (result?.IsValid == true)
                {
                    return new TelegramWebAppValidationResult
                    {
                        IsValid = true,
                        Data = result.Data
                    };
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = error?.Error ?? "Validation failed"
                };
            }
        }
        catch (Exception ex)
        {
            return new TelegramWebAppValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Error during validation: {ex.Message}"
            };
        }

        return new TelegramWebAppValidationResult
        {
            IsValid = false,
            ErrorMessage = "Unknown error during validation"
        };
    }

    public string? GetTgWebAppDataFromQuery()
    {
        var uri = new Uri(_navigationManager.Uri);
        if (string.IsNullOrEmpty(uri.Query))
        {
            return null;
        }

        var queryParams = uri.Query.TrimStart('?')
            .Split('&')
            .Select(param => param.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]),
                parts => Uri.UnescapeDataString(parts[1])
            );

        return queryParams.TryGetValue("tgWebAppData", out var value) ? value : null;
    }
}

internal class TelegramWebAppValidationResponse
{
    public bool IsValid { get; set; }
    public TelegramWebAppData? Data { get; set; }
}

internal class ErrorResponse
{
    public string? Error { get; set; }
}

