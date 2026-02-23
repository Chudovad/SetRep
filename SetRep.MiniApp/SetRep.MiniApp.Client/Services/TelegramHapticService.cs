using Microsoft.JSInterop;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Реализация сервиса тактильной отдачи в Telegram WebApp.
/// </summary>
public class TelegramHapticService : ITelegramHapticService
{
    private readonly IJSRuntime _jsRuntime;

    public TelegramHapticService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SelectionChangedAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("telegramHapticHelper.selectionChanged");
        }
        catch
        {
            // Вне Telegram WebApp хелпер недоступен — игнорируем
        }
    }

    public async Task NotificationSuccessAsync()
    {
        await NotificationAsync();
    }

    public async Task NotificationErrorAsync()
    {
        await NotificationAsync("error");
    }

    public async Task NotificationWarningAsync()
    {
        await NotificationAsync("warning");
    }

    public async Task ImpactHeavyAsync()
    {
        await ImpactAsync("heavy");
    }

    public async Task ImpactLightAsync()
    {
        await ImpactAsync("light");
    }

    public async Task ImpactMediumAsync()
    {
        await ImpactAsync("medium");
    }

    public async Task ImpactRigidAsync()
    {
        await ImpactAsync("rigid");
    }

    public async Task ImpactSoftAsync()
    {
        await ImpactAsync("soft");
    }

    private async Task NotificationAsync(string type = "success")
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("telegramHapticHelper.notificationOccurred", type);
        }
        catch
        {
            // Вне Telegram WebApp хелпер недоступен — игнорируем
        }
    }

    private async Task ImpactAsync(string style)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("telegramHapticHelper.impactOccurred", style);
        }
        catch
        {
            // Вне Telegram WebApp хелпер недоступен — игнорируем
        }
    }
}
