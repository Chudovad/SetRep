using System.Text.Json.Serialization;

namespace SetRep.Shared.Telegram
{
    public class TelegramUser
    {
        [JsonPropertyName("userId")]
        public Guid Id { get; set; }

        [JsonPropertyName("id")]
        public long TelegramId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("language_code")]
        public string? LanguageCode { get; set; }

        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("is_bot")]
        public bool? IsBot { get; set; }

        [JsonPropertyName("photo_url")]
        public string? PhotoUrl { get; set; }
    }
}