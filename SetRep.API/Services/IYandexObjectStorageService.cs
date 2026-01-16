namespace SetRep.API.Services;

public interface IYandexObjectStorageService
{
    /// <summary>
    /// Загружает файл в Yandex Object Storage
    /// </summary>
    /// <param name="fileStream">Поток файла</param>
    /// <param name="fileName">Имя файла</param>
    /// <param name="contentType">MIME тип файла</param>
    /// <returns>URL загруженного файла</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Удаляет файл из Yandex Object Storage
    /// </summary>
    /// <param name="fileUrl">URL файла для удаления</param>
    Task DeleteFileAsync(string fileUrl);
}
