namespace SetRep.Shared.Telegram;

public class TelegramUser
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? LanguageCode { get; set; }
    public bool IsPremium { get; set; }
    public bool? IsBot { get; set; }
    public string? PhotoUrl { get; set; }
}

