using SetRep.Core.Repositories;

namespace SetRep.API.Services;

/// <summary>
/// Фоновый сервис для автоматического завершения тренировок в конце дня
/// </summary>
public class WorkoutAutoCompletionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WorkoutAutoCompletionService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Проверка каждый час

    public WorkoutAutoCompletionService(
        IServiceProvider serviceProvider,
        ILogger<WorkoutAutoCompletionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AutoCompleteWorkoutsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при автоматическом завершении тренировок");
            }

            // Ждем до следующей проверки
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task AutoCompleteWorkoutsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var workoutHistoryRepository = scope.ServiceProvider.GetRequiredService<IWorkoutHistoryRepository>();

        // Получаем все активные тренировки, начатые до начала текущего дня
        var today = DateTime.UtcNow.Date;
        var activeWorkouts = await workoutHistoryRepository.GetActiveWorkoutsStartedBeforeAsync(today);

        if (!activeWorkouts.Any())
        {
            return;
        }

        _logger.LogInformation("Найдено {Count} активных тренировок для автоматического завершения", activeWorkouts.Count());

        foreach (var workout in activeWorkouts)
        {
            try
            {
                // Завершаем тренировку в конце дня, когда она была начата
                // Устанавливаем EndTime на 23:59:59 дня начала тренировки
                var endOfDay = workout.StartTime.Date.AddDays(1).AddSeconds(-1);
                
                workout.EndTime = endOfDay;
                workout.UpdatedAt = DateTime.UtcNow;

                await workoutHistoryRepository.UpdateAsync(workout);
                await workoutHistoryRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Тренировка {WorkoutHistoryId} автоматически завершена. Начата: {StartTime}, Завершена: {EndTime}",
                    workout.Id, workout.StartTime, workout.EndTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Ошибка при автоматическом завершении тренировки {WorkoutHistoryId}", 
                    workout.Id);
            }
        }
    }
}

