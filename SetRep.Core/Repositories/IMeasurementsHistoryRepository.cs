using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с историей измерений
/// </summary>
public interface IMeasurementsHistoryRepository : IRepository<MeasurementsHistory>
{
    /// <summary>
    /// Получить историю измерений пользователя
    /// </summary>
    Task<IEnumerable<MeasurementsHistory>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить историю измерений пользователя за период
    /// </summary>
    Task<IEnumerable<MeasurementsHistory>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Получить последнюю запись измерений пользователя
    /// </summary>
    Task<MeasurementsHistory?> GetLastByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить запись измерений пользователя по Notes
    /// </summary>
    Task<MeasurementsHistory?> GetByUserIdAndNotesAsync(Guid userId, string notes);
}

