namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для управления кнопкой "Назад" в Telegram WebApp
/// </summary>
public interface ITelegramBackButtonService
{
    /// <summary>
    /// Показывает кнопку "Назад" и устанавливает обработчик для возврата на предыдущую страницу
    /// </summary>
    Task ShowAsync();

    /// <summary>
    /// Скрывает кнопку "Назад"
    /// </summary>
    Task HideAsync();
}

