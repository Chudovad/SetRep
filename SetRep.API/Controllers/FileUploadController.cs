using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetRep.API.Options;
using SetRep.API.Services;
using System.Security.Claims;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IYandexObjectStorageService _storageService;
    private readonly FileUploadOptions _fileUploadOptions;
    private readonly ILogger<FileUploadController> _logger;
    private static readonly Dictionary<Guid, int> _userUploadCounts = new();
    private static DateTime _lastResetTime = DateTime.UtcNow;
    private static readonly Lock _lockObject = new();

    public FileUploadController(
        IYandexObjectStorageService storageService,
        IOptions<FileUploadOptions> fileUploadOptions,
        ILogger<FileUploadController> logger)
    {
        _storageService = storageService;
        _fileUploadOptions = fileUploadOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Загрузить файл изображения
    /// </summary>
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            var maxFileSize = _fileUploadOptions.MaxFileSizeBytes;
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Не удалось определить пользователя");
            }

            if (!CheckUploadLimit(userId))
            {
                return BadRequest(new { error = "Превышен лимит загрузок. Попробуйте позже." });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "Файл не был загружен" });
            }

            if (file.Length > maxFileSize)
            {
                return BadRequest(new { error = $"Размер файла превышает {maxFileSize / 1024 / 1024} МБ" });
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new { error = "Разрешены только изображения (JPEG, PNG, GIF, WebP)" });
            }

            using var originalStream = file.OpenReadStream();
            using var compressedStream = await CompressImageAsync(originalStream, file.ContentType);
            
            // Определяем новый content type (всегда JPEG после сжатия, кроме WebP)
            var outputContentType = file.ContentType.ToLower() == "image/webp" ? "image/webp" : "image/jpeg";
            var outputFileName = Path.ChangeExtension(file.FileName, outputContentType == "image/webp" ? ".webp" : ".jpg");
            
            var fileUrl = await _storageService.UploadFileAsync(compressedStream, outputFileName, outputContentType);

            IncrementUploadCount(userId);

            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла");
            return StatusCode(500, new { error = "Ошибка при загрузке файла" });
        }
    }

    /// <summary>
    /// Удалить файл изображения
    /// </summary>
    [HttpDelete("image")]
    public async Task<IActionResult> DeleteImage([FromQuery] string url)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Не удалось определить пользователя");
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest(new { error = "URL файла не указан" });
            }

            await _storageService.DeleteFileAsync(url);

            _logger.LogInformation("Пользователь {UserId} удалил файл {Url}", userId, url);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении файла");
            return StatusCode(500, new { error = "Ошибка при удалении файла" });
        }
    }

    private bool CheckUploadLimit(Guid userId)
    {
        lock (_lockObject)
        {
            if (DateTime.UtcNow - _lastResetTime > TimeSpan.FromHours(1))
            {
                _userUploadCounts.Clear();
                _lastResetTime = DateTime.UtcNow;
            }

            var maxUploads = _fileUploadOptions.MaxUploadsPerUser;
            if (_userUploadCounts.TryGetValue(userId, out var count))
            {
                return count < maxUploads;
            }

            return true;
        }
    }

    private void IncrementUploadCount(Guid userId)
    {
        lock (_lockObject)
        {
            if (_userUploadCounts.TryGetValue(userId, out int value))
            {
                _userUploadCounts[userId] = ++value;
            }
            else
            {
                _userUploadCounts[userId] = 1;
            }
        }
    }

    /// <summary>
    /// Сжимает изображение до качества ~80% и размера 300-800 КБ
    /// </summary>
    private async Task<MemoryStream> CompressImageAsync(Stream inputStream, string contentType)
    {
        const int minSizeBytes = 300 * 1024; // 300 КБ
        const int maxSizeBytes = 800 * 1024; // 800 КБ
        const int initialQuality = 80; // Начальное качество 80%

        using var image = await Image.LoadAsync(inputStream);
        
        // Определяем формат вывода
        var isWebP = contentType.ToLower() == "image/webp";
        
        // Для WebP сохраняем в WebP, для остальных - конвертируем в JPEG
        var outputStream = new MemoryStream();
        int quality = initialQuality;
        int attempts = 0;
        const int maxAttempts = 10;

        while (attempts < maxAttempts)
        {
            outputStream.SetLength(0);
            outputStream.Position = 0;

            if (isWebP)
            {
                var encoder = new WebpEncoder
                {
                    Quality = quality,
                    Method = WebpEncodingMethod.BestQuality
                };
                await image.SaveAsync(outputStream, encoder);
            }
            else
            {
                var encoder = new JpegEncoder
                {
                    Quality = quality
                };
                await image.SaveAsync(outputStream, encoder);
            }

            var compressedSize = outputStream.Length;

            // Если размер в нужном диапазоне или меньше минимального (для очень маленьких изображений), возвращаем результат
            if (compressedSize <= maxSizeBytes)
            {
                _logger.LogInformation(
                    "Изображение сжато до {Size} байт с качеством {Quality}%",
                    compressedSize, quality);
                break;
            }

            // Если размер больше максимального, уменьшаем качество
            quality = Math.Max(50, quality - 5); // Минимум 50%
            attempts++;
        }

        outputStream.Position = 0;
        var result = new MemoryStream();
        await outputStream.CopyToAsync(result);
        result.Position = 0;
        
        return result;
    }
}
