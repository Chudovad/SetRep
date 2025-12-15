using Microsoft.JSInterop;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Реализация сервиса для управления кнопкой "Назад" в Telegram WebApp
/// </summary>
public class TelegramBackButtonService : ITelegramBackButtonService, IDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<TelegramBackButtonService>? _logger;
    private DotNetObjectReference<TelegramBackButtonService>? _dotNetRef;
    private bool _isInitialized = false;

    public TelegramBackButtonService(
        IJSRuntime jsRuntime,
        ILogger<TelegramBackButtonService>? logger = null)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task ShowAsync()
    {
        try
        {
            // Инициализируем обработчик клика только один раз
            if (!_isInitialized)
            {
                await InitializeOnClickHandler();
                _isInitialized = true;
            }

            var startTime = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(1);

            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    var isReady = await _jsRuntime.InvokeAsync<bool>("telegramBackButtonHelper.isReady");

                    if (isReady)
                    {
                        await _jsRuntime.InvokeVoidAsync("telegramBackButtonHelper.show");
                        _logger?.LogDebug("Кнопка 'Назад' показана");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogDebug(ex, "Ожидание загрузки Telegram WebApp...");
                }

                await Task.Delay(50);
            }

            _logger?.LogWarning("Telegram WebApp не загрузился вовремя, кнопка 'Назад' не показана");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при показе кнопки 'Назад'");
        }
    }

    private async Task InitializeOnClickHandler()
    {
        try
        {
            var startTime = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(1);

            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    var isReady = await _jsRuntime.InvokeAsync<bool>("telegramBackButtonHelper.isReady");

                    if (isReady)
                    {
                        if (_dotNetRef == null)
                        {
                            _dotNetRef = DotNetObjectReference.Create(this);
                        }

                        await _jsRuntime.InvokeVoidAsync("telegramBackButtonHelper.onClick", _dotNetRef);
                        _logger?.LogDebug("Обработчик клика на кнопку 'Назад' установлен");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogDebug(ex, "Ожидание загрузки Telegram WebApp...");
                }

                await Task.Delay(50);
            }

            _logger?.LogWarning("Telegram WebApp не загрузился вовремя, обработчик клика не установлен");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при установке обработчика клика на кнопку 'Назад'");
        }
    }

    public async Task HideAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("telegramBackButtonHelper.hide");
            _logger?.LogDebug("Кнопка 'Назад' скрыта");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при скрытии кнопки 'Назад'");
        }
    }

    [JSInvokable]
    public async Task OnBackButtonClick()
    {
        try
        {
            // Используем JavaScript для возврата на предыдущую страницу
            await _jsRuntime.InvokeVoidAsync("eval", "window.history.back()");
            _logger?.LogDebug("Выполнен возврат на предыдущую страницу");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при возврате на предыдущую страницу");
        }
    }

    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }
}

