using Microsoft.JSInterop;
using SetRep.Shared.Telegram;
using System.Net.Http.Json;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Реализация сервиса для работы с Telegram WebApp данными
/// </summary>
public class TelegramWebAppService : ITelegramWebAppService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelegramWebAppService> _logger;

    public TelegramWebAppService(
        IJSRuntime jsRuntime,
        HttpClient httpClient,
        ILogger<TelegramWebAppService> logger)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetTelegramWebAppDataAsync()
    {
        try
        {
            // Сначала пробуем использовать наш JavaScript helper
            try
            {
                var tgWebAppData = await _jsRuntime.InvokeAsync<string>("getTelegramWebAppData");
                if (!string.IsNullOrWhiteSpace(tgWebAppData))
                {
                    return tgWebAppData;
                }
            }
            catch (JSException)
            {
                // Если функция не найдена, пробуем напрямую
            }

            // Пробуем получить из URL параметров
            var tgWebAppDataFromUrl = await _jsRuntime.InvokeAsync<string>("eval",
                "new URLSearchParams(window.location.search).get('tgWebAppData')");
            
            if (!string.IsNullOrWhiteSpace(tgWebAppDataFromUrl))
            {
                return tgWebAppDataFromUrl;
            }

            // Пробуем получить из Telegram WebApp API
            var tgWebAppDataFromTelegram = await _jsRuntime.InvokeAsync<string>("eval",
                "window.Telegram?.WebApp?.initData || null");
            
            if (!string.IsNullOrWhiteSpace(tgWebAppDataFromTelegram))
            {
                return tgWebAppDataFromTelegram;
            }

            _logger.LogWarning("tgWebAppData не найден в URL или window.Telegram.WebApp.initData");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении tgWebAppData");
            return null;
        }
    }

    public async Task<TelegramWebAppValidationResult> ValidateAsync(string tgWebAppData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tgWebAppData))
            {
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "tgWebAppData пуст"
                };
            }

            var request = new { TgWebAppData = tgWebAppData };
            var response = await _httpClient.PostAsJsonAsync("api/TelegramWebApp/validate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TelegramWebAppValidationResponse>();
                if (result != null && result.IsValid && result.Data != null)
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
                ErrorResponse? errorResponse = null;
                try
                {
                    var contentLength = response.Content.Headers.ContentLength;
                    if (contentLength.HasValue && contentLength.Value > 0)
                    {
                        var contentType = response.Content.Headers.ContentType?.MediaType;
                        if (contentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось десериализовать ответ об ошибке");
                }
                
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = errorResponse?.Error ?? "Неавторизован"
                };
            }
            else
            {
                ErrorResponse? errorResponse = null;
                try
                {
                    var contentLength = response.Content.Headers.ContentLength;
                    if (contentLength.HasValue && contentLength.Value > 0)
                    {
                        var contentType = response.Content.Headers.ContentType?.MediaType;
                        if (contentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось десериализовать ответ об ошибке");
                }
                
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = errorResponse?.Error ?? $"Ошибка валидации: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при валидации Telegram WebApp данных");
            return new TelegramWebAppValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Ошибка валидации: {ex.Message}"
            };
        }

        return new TelegramWebAppValidationResult
        {
            IsValid = false,
            ErrorMessage = "Неизвестная ошибка валидации"
        };
    }

    public async Task<TelegramWebAppValidationResult> GetAndValidateAsync()
    {
        var tgWebAppData = await GetTelegramWebAppDataAsync();
        if (tgWebAppData == null)
        {
            return new TelegramWebAppValidationResult
            {
                IsValid = false,
                ErrorMessage = "tgWebAppData не найден"
            };
        }

        return await ValidateAsync(tgWebAppData);
    }

    private class TelegramWebAppValidationResponse
    {
        public bool IsValid { get; set; }
        public TelegramWebAppData? Data { get; set; }
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}

