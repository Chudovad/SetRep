namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// Реализация сервиса для управления состоянием инициализации приложения
/// </summary>
public class AppInitializationService : IAppInitializationService
{
    private bool _isInitializing = true;
    private string? _currentStep;

    public bool IsInitializing
    {
        get => _isInitializing;
        private set
        {
            if (_isInitializing != value)
            {
                _isInitializing = value;
                InitializationStateChanged?.Invoke();
            }
        }
    }

    public string? CurrentStep
    {
        get => _currentStep;
        private set
        {
            if (_currentStep != value)
            {
                _currentStep = value;
                InitializationStateChanged?.Invoke();
            }
        }
    }

    public event Action? InitializationStateChanged;

    public void SetInitializing(bool isInitializing, string? step = null)
    {
        IsInitializing = isInitializing;
        // Шаги больше не отображаются, но оставляем для совместимости
        CurrentStep = step;
    }
}

