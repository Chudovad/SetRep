using ImageMagick;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SetRep.MiniApp.Options;

namespace SetRep.MiniApp.Services;

/// <summary>
/// Реализация сервиса для конвертации изображений с использованием Magick.NET
/// </summary>
public class ImageConversionService : IImageConversionService
{
    private readonly ILogger<ImageConversionService>? _logger;
    private readonly long _maxFileSize;

    public ImageConversionService(
        IOptions<FileUploadOptions> fileUploadOptions,
        ILogger<ImageConversionService>? logger = null)
    {
        _logger = logger;
        _maxFileSize = fileUploadOptions.Value.MaxFileSizeBytes;
    }

    public bool IsHeicFormat(IBrowserFile file)
    {
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var fileName = file.Name?.ToLowerInvariant() ?? string.Empty;

        return contentType.Contains("heic") ||
               contentType.Contains("heif") ||
               fileName.EndsWith(".heic", StringComparison.OrdinalIgnoreCase) ||
               fileName.EndsWith(".heif", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Stream?> ConvertToJpegIfNeededAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            // Если файл не в формате HEIC/HEIF, возвращаем null (конвертация не требуется)
            if (!IsHeicFormat(file))
            {
                return null;
            }

            _logger?.LogInformation("Начало конвертации HEIC файла: {FileName}", file.Name);

            // Читаем исходный файл
            using var sourceStream = file.OpenReadStream(maxAllowedSize: _maxFileSize);
            using var memoryStream = new MemoryStream();
            await sourceStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            // Конвертируем HEIC в JPEG с помощью Magick.NET
            using var image = new MagickImage(memoryStream);
            
            // Настраиваем параметры конвертации
            image.Format = MagickFormat.Jpeg;
            image.Quality = 90; // Качество JPEG (0-100)

            // Создаем новый поток для результата
            var resultStream = new MemoryStream();
            await image.WriteAsync(resultStream, cancellationToken);
            resultStream.Position = 0;

            _logger?.LogInformation("Конвертация HEIC файла завершена успешно: {FileName}", file.Name);

            return resultStream;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при конвертации HEIC файла: {FileName}", file.Name);
            throw;
        }
    }
}

