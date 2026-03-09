using Microsoft.AspNetCore.Components.Forms;

namespace SetRep.MiniApp.Services;

/// <summary>
/// Сервис для конвертации изображений в поддерживаемые браузером форматы
/// </summary>
public interface IImageConversionService
{
    /// <summary>
    /// Конвертирует изображение в JPEG формат, если оно в формате HEIC/HEIF
    /// </summary>
    /// <param name="file">Файл изображения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток с конвертированным изображением в формате JPEG, или null если конвертация не требуется</returns>
    Task<Stream?> ConvertToJpegIfNeededAsync(IBrowserFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, является ли файл форматом HEIC/HEIF
    /// </summary>
    /// <param name="file">Файл для проверки</param>
    /// <returns>True, если файл в формате HEIC/HEIF</returns>
    bool IsHeicFormat(IBrowserFile file);
}

