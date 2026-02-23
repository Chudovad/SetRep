namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис тактильной отдачи (Haptic Feedback) в Telegram WebApp.
/// Вызовы безопасны вне Telegram — просто не выполняются.
/// </summary>
public interface ITelegramHapticService
{
    /// <summary>Воздействие: heavy</summary>
    Task ImpactHeavyAsync();

    /// <summary>Воздействие: light</summary>
    Task ImpactLightAsync();

    /// <summary>Воздействие: medium</summary>
    Task ImpactMediumAsync();

    /// <summary>Воздействие: rigid</summary>
    Task ImpactRigidAsync();

    /// <summary>Воздействие: soft</summary>
    Task ImpactSoftAsync();

    /// <summary>Успешное уведомление</summary>
    Task NotificationSuccessAsync();

    /// <summary>Уведомление ошибки</summary>
    Task NotificationErrorAsync();

    /// <summary>Уведомление внимание</summary>
    Task NotificationWarningAsync();

    /// <summary>Тактильная отдача при смене выбора (selection changed)</summary>
    Task SelectionChangedAsync();
}
