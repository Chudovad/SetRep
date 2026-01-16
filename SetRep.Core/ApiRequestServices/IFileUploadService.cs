namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для загрузки файлов
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Загрузить изображение
    /// </summary>
    /// <param name="file">Файл для загрузки</param>
    /// <returns>URL загруженного файла или null в случае ошибки</returns>
    Task<string?> UploadImageAsync(Stream fileStream, string fileName, string contentType);
}
