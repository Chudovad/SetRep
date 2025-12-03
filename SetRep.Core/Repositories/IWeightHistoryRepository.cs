using SetRep.Core.Entities;

namespace SetRep.Core.Repositories;

/// <summary>
/// Репозиторий для работы с историей веса
/// </summary>
public interface IWeightHistoryRepository : IRepository<WeightHistory>
{
    /// <summary>
    /// Получить историю веса пользователя
    /// </summary>
    Task<IEnumerable<WeightHistory>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить историю веса пользователя за период
    /// </summary>
    Task<IEnumerable<WeightHistory>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Получить последнюю запись веса пользователя
    /// </summary>
    Task<WeightHistory?> GetLastByUserIdAsync(Guid userId);
}

