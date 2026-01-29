using System.Net.Http.Headers;
using System.Net.Http.Json;
using SetRep.Core.ApiRequestServices;
using SetRep.MiniApp.Client.Extensions;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для загрузки файлов
/// </summary>
public class FileUploadService : IFileUploadService
{
    private readonly HttpClient _httpClient;
    private readonly ITelegramWebAppStateService _telegramWebAppStateService;
    private readonly ILogger<FileUploadService>? _logger;

    public FileUploadService(
        HttpClient httpClient,
        ITelegramWebAppStateService telegramWebAppStateService,
        ILogger<FileUploadService>? logger = null)
    {
        _httpClient = httpClient;
        _telegramWebAppStateService = telegramWebAppStateService;
        _logger = logger;
    }

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(streamContent, "file", fileName);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/fileupload/image")
            {
                Content = content
            };
            request.AddBearerToken(_telegramWebAppStateService);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
                return result?.Url;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogError("Ошибка загрузки файла. Status: {Status}, Error: {Error}", 
                    response.StatusCode, errorContent);
                
                // Пытаемся извлечь сообщение об ошибке из JSON
                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    _logger?.LogError("Ошибка: {Error}", errorObj?.Error);
                }
                catch
                {
                    // Игнорируем ошибку парсинга
                }
                
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при загрузке файла {FileName}", fileName);
            return null;
        }
    }

    public async Task<bool> DeleteImageAsync(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/fileupload/image?url={Uri.EscapeDataString(url)}");
            request.AddBearerToken(_telegramWebAppStateService);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _logger?.LogInformation("Изображение успешно удалено: {Url}", url);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogError("Ошибка удаления файла. Status: {Status}, Error: {Error}", 
                    response.StatusCode, errorContent);
                
                // Пытаемся извлечь сообщение об ошибке из JSON
                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    _logger?.LogError("Ошибка: {Error}", errorObj?.Error);
                }
                catch
                {
                    // Игнорируем ошибку парсинга
                }
                
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при удалении файла {Url}", url);
            return false;
        }
    }

    private class UploadResponse
    {
        public string? Url { get; set; }
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}
