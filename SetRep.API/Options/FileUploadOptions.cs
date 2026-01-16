namespace SetRep.API.Options;

/// <summary>
/// Настройки загрузки файлов
/// </summary>
public class FileUploadOptions
{
    public const string SectionName = "FileUpload";

    /// <summary>
    /// Максимальный размер файла в байтах
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5242880; // 5 МБ по умолчанию

    /// <summary>
    /// Максимальное количество загрузок на пользователя
    /// </summary>
    public int MaxUploadsPerUser { get; set; } = 10;
}

