namespace SetRep.Shared.Telegram;

public class TelegramWebAppData
{
    public string QueryId { get; set; } = string.Empty;
    public TelegramUser User { get; set; } = new();
    public long? AuthDate { get; set; }
    public string? Hash { get; set; }
    public string? StartParam { get; set; }
    public string? ChatType { get; set; }
    public long? ChatInstance { get; set; }
    public string? Token { get; set; }
}

public class TelegramWebAppValidationResult
{
    public bool IsValid { get; set; }
    public TelegramWebAppData? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

