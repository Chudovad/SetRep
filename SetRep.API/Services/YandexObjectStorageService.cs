using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text.RegularExpressions;

namespace SetRep.API.Services;

public class YandexObjectStorageService : IYandexObjectStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _serviceUrl;
    private readonly ILogger<YandexObjectStorageService> _logger;

    public YandexObjectStorageService(
        IConfiguration configuration,
        ILogger<YandexObjectStorageService> logger)
    {
        _logger = logger;
        
        var accessKey = configuration["YANDEX_OBJECT_STORAGE_ACCESS_KEY"]
            ?? configuration["YandexObjectStorage:AccessKey"]
            ?? throw new InvalidOperationException("YandexObjectStorage:AccessKey is not configured");
        var secretKey = configuration["YANDEX_OBJECT_STORAGE_SECRET_KEY"]
            ?? configuration["YandexObjectStorage:SecretKey"]
            ?? throw new InvalidOperationException("YandexObjectStorage:SecretKey is not configured");
        var endpoint = configuration["YANDEX_OBJECT_STORAGE_ENDPOINT"]
            ?? configuration["YandexObjectStorage:Endpoint"]
            ?? throw new InvalidOperationException("YandexObjectStorage:Endpoint is not configured");
        _bucketName = configuration["YANDEX_OBJECT_STORAGE_BUCKET_NAME"]
            ?? configuration["YandexObjectStorage:BucketName"]
            ?? throw new InvalidOperationException("YandexObjectStorage:BucketName is not configured");
        _serviceUrl = configuration["YANDEX_OBJECT_STORAGE_SERVICE_URL"]
            ?? configuration["YandexObjectStorage:ServiceUrl"]
            ?? throw new InvalidOperationException("YandexObjectStorage:ServiceUrl is not configured");

        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true,
        };

        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        _s3Client = new AmazonS3Client(credentials, config);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{SanitizeFileName(fileName)}";
            
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueFileName,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);

            var fileUrl = $"{_serviceUrl}/{_bucketName}/{uniqueFileName}";
            
            _logger.LogInformation("Файл {FileName} успешно загружен в Yandex Object Storage. URL: {FileUrl}", fileName, fileUrl);
            
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла {FileName} в Yandex Object Storage", fileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            var key = ExtractKeyFromUrl(fileUrl);
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Не удалось извлечь ключ из URL: {FileUrl}", fileUrl);
                return;
            }

            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
            
            _logger.LogInformation("Файл {Key} успешно удален из Yandex Object Storage", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении файла {FileUrl} из Yandex Object Storage", fileUrl);
            throw;
        }
    }

    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        
        if (sanitized.Length > 100)
        {
            var extension = Path.GetExtension(sanitized);
            sanitized = sanitized.Substring(0, 100 - extension.Length) + extension;
        }
        
        return sanitized;
    }

    private string? ExtractKeyFromUrl(string fileUrl)
    {
        // Извлекаем ключ из URL вида: https://storage.yandexcloud.net/bucket-name/file-key
        var match = Regex.Match(fileUrl, $@"{Regex.Escape(_serviceUrl)}/{Regex.Escape(_bucketName)}/(.+)");
        return match.Success ? match.Groups[1].Value : null;
    }
}
