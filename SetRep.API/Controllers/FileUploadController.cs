using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.API.Services;
using System.Security.Claims;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IYandexObjectStorageService _storageService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileUploadController> _logger;
    private static readonly Dictionary<Guid, int> _userUploadCounts = new();
    private static DateTime _lastResetTime = DateTime.UtcNow;
    private static readonly object _lockObject = new();

    public FileUploadController(
        IYandexObjectStorageService storageService,
        IConfiguration configuration,
        ILogger<FileUploadController> logger)
    {
        _storageService = storageService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Загрузить файл изображения
    /// </summary>
    [HttpPost("image")]
    [RequestSizeLimit(1048576)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
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

            var maxFileSize = _configuration.GetValue<long>("FILE_UPLOAD_MAX_SIZE_BYTES", 
                _configuration.GetValue<long>("FileUpload:MaxFileSizeBytes", 1048576));
            if (file.Length > maxFileSize)
            {
                return BadRequest(new { error = $"Размер файла превышает {maxFileSize / 1024} КБ" });
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new { error = "Разрешены только изображения (JPEG, PNG, GIF, WebP)" });
            }

            using var stream = file.OpenReadStream();
            var fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

            IncrementUploadCount(userId);

            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла");
            return StatusCode(500, new { error = "Ошибка при загрузке файла" });
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

            var maxUploads = _configuration.GetValue<int>("FILE_UPLOAD_MAX_UPLOADS_PER_USER",
                _configuration.GetValue<int>("FileUpload:MaxUploadsPerUser", 10));
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
            if (_userUploadCounts.ContainsKey(userId))
            {
                _userUploadCounts[userId]++;
            }
            else
            {
                _userUploadCounts[userId] = 1;
            }
        }
    }
}
