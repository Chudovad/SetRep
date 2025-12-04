using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SetRep.Shared.Telegram;

namespace SetRep.API.Services;

public class TelegramWebAppValidationService : ITelegramWebAppValidationService
{
    public TelegramWebAppValidationResult Validate(string tgWebAppData, string botToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tgWebAppData))
            {
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "tgWebAppData is empty"
                };
            }

            if (string.IsNullOrWhiteSpace(botToken))
            {
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "BotToken is not configured"
                };
            }

            // Парсим query string
            var parameters = ParseQueryString(tgWebAppData);
            
            if (!parameters.TryGetValue("hash", out var hash) || string.IsNullOrWhiteSpace(hash))
            {
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Hash parameter is missing"
                };
            }

            // Проверяем подпись
            if (!ValidateSignature(parameters, hash, botToken))
            {
                return new TelegramWebAppValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid signature"
                };
            }

            // Проверяем время (auth_date не должен быть старше 24 часов)
            if (parameters.TryGetValue("auth_date", out var authDateStr) && 
                long.TryParse(authDateStr, out var authDate))
            {
                var authDateTime = DateTimeOffset.FromUnixTimeSeconds(authDate);
                if (DateTimeOffset.UtcNow - authDateTime > TimeSpan.FromHours(24))
                {
                    return new TelegramWebAppValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Data is too old (more than 24 hours)"
                    };
                }
            }

            // Парсим данные пользователя
            var webAppData = ParseWebAppData(parameters);

            return new TelegramWebAppValidationResult
            {
                IsValid = true,
                Data = webAppData
            };
        }
        catch (Exception ex)
        {
            return new TelegramWebAppValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Validation error: {ex.Message}"
            };
        }
    }

    private Dictionary<string, string> ParseQueryString(string queryString)
    {
        var parameters = new Dictionary<string, string>();
        var pairs = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                parameters[key] = value;
            }
        }

        return parameters;
    }

    private bool ValidateSignature(Dictionary<string, string> parameters, string receivedHash, string botToken)
    {
        // Исключаем hash из проверки и сортируем по ключу
        var dataToCheck = parameters
            .Where(p => p.Key != "hash")
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .Select(p => $"{p.Key}={p.Value}")
            .ToArray();

        var dataCheckString = string.Join("\n", dataToCheck);

        // Генерируем секретный ключ согласно официальной схеме Telegram:
        // secret_key = HMACSHA256("WebAppData", bot_token)
        byte[] secretKey;
        using (var hmacForSecret = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData")))
        {
            secretKey = hmacForSecret.ComputeHash(Encoding.UTF8.GetBytes(botToken));
        }

        // Вычисляем HMAC-SHA256 для проверки подписи
        byte[] computedHash;
        using (var hmac = new HMACSHA256(secretKey))
        {
            computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        }

        // Преобразуем вычисленный хэш в hex строку
        var computedHashHex = Convert.ToHexString(computedHash).ToLowerInvariant();

        // Преобразуем полученный хэш из hex строки в байты для сравнения
        byte[] receivedHashBytes;
        try
        {
            receivedHashBytes = Convert.FromHexString(receivedHash);
        }
        catch
        {
            // Если не удалось преобразовать, сравниваем как строки
            return string.Equals(computedHashHex, receivedHash, StringComparison.OrdinalIgnoreCase);
        }

        // Сравниваем хэши побайтово (более безопасно)
        return computedHash.SequenceEqual(receivedHashBytes);
    }

    private TelegramWebAppData ParseWebAppData(Dictionary<string, string> parameters)
    {
        var data = new TelegramWebAppData
        {
            Hash = parameters.GetValueOrDefault("hash"),
            AuthDate = parameters.TryGetValue("auth_date", out var authDate) && long.TryParse(authDate, out var authDateLong) 
                ? authDateLong 
                : null,
            QueryId = parameters.GetValueOrDefault("query_id") ?? string.Empty,
            StartParam = parameters.GetValueOrDefault("start_param"),
            ChatType = parameters.GetValueOrDefault("chat_type"),
            ChatInstance = parameters.TryGetValue("chat_instance", out var chatInstance) && long.TryParse(chatInstance, out var chatInstanceLong)
                ? chatInstanceLong
                : null
        };

        // Парсим данные пользователя
        if (parameters.TryGetValue("user", out var userJson))
        {
            try
            {
                var user = JsonSerializer.Deserialize<TelegramUser>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (user != null)
                {
                    data.User = user;
                }
            }
            catch
            {
                // Если не удалось распарсить JSON, пробуем парсить отдельные поля
                if (parameters.TryGetValue("id", out var idStr) && long.TryParse(idStr, out var id))
                {
                    data.User.Id = id;
                    data.User.FirstName = parameters.GetValueOrDefault("first_name") ?? string.Empty;
                    data.User.LastName = parameters.GetValueOrDefault("last_name");
                    data.User.Username = parameters.GetValueOrDefault("username");
                    data.User.LanguageCode = parameters.GetValueOrDefault("language_code");
                    if (parameters.TryGetValue("is_premium", out var isPremiumStr) && bool.TryParse(isPremiumStr, out var isPremium))
                    {
                        data.User.IsPremium = isPremium;
                    }
                    if (parameters.TryGetValue("is_bot", out var isBotStr) && bool.TryParse(isBotStr, out var isBot))
                    {
                        data.User.IsBot = isBot;
                    }
                    data.User.PhotoUrl = parameters.GetValueOrDefault("photo_url");
                }
            }
        }
        else
        {
            // Парсим отдельные поля пользователя
            if (parameters.TryGetValue("id", out var idStr) && long.TryParse(idStr, out var id))
            {
                data.User.Id = id;
                data.User.FirstName = parameters.GetValueOrDefault("first_name") ?? string.Empty;
                data.User.LastName = parameters.GetValueOrDefault("last_name");
                data.User.Username = parameters.GetValueOrDefault("username");
                data.User.LanguageCode = parameters.GetValueOrDefault("language_code");
                if (parameters.TryGetValue("is_premium", out var isPremiumStr) && bool.TryParse(isPremiumStr, out var isPremium))
                {
                    data.User.IsPremium = isPremium;
                }
                if (parameters.TryGetValue("is_bot", out var isBotStr) && bool.TryParse(isBotStr, out var isBot))
                {
                    data.User.IsBot = isBot;
                }
                data.User.PhotoUrl = parameters.GetValueOrDefault("photo_url");
            }
        }

        return data;
    }
}

