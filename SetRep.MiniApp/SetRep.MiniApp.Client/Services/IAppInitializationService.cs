namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Сервис для управления состоянием инициализации приложения
/// </summary>
public interface IAppInitializationService
{
    /// <summary>
    /// Текущий шаг инициализации
    /// </summary>
    string? CurrentStep { get; }

    /// <summary>
    /// Флаг, указывающий, идет ли инициализация
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Событие изменения состояния инициализации
    /// </summary>
    event Action? InitializationStateChanged;

    /// <summary>
    /// Устанавливает состояние инициализации
    /// </summary>
    void SetInitializing(bool isInitializing, string? step = null);
}

