using SetRep.Core.Entities;

namespace SetRep.Core.ApiRequestServices;

/// <summary>
/// Сервис для работы с историей измерений пользователя
/// </summary>
public interface IMeasurementsHistoryService
{
    /// <summary>
    /// Получить все записи измерений пользователя
    /// </summary>
    Task<List<MeasurementsHistory>?> GetMeasurementsHistoryByUserIdAsync(Guid userId);

    /// <summary>
    /// Получить запись измерений по ID
    /// </summary>
    Task<MeasurementsHistory?> GetMeasurementsHistoryByIdAsync(Guid id);

    /// <summary>
    /// Создать новую запись измерений
    /// </summary>
    Task<MeasurementsHistory?> CreateMeasurementsHistoryAsync(Shared.DTO.CreateMeasurementsHistoryDto dto);

    /// <summary>
    /// Обновить запись измерений
    /// </summary>
    Task<bool> UpdateMeasurementsHistoryAsync(Guid id, Shared.DTO.CreateMeasurementsHistoryDto dto);

    /// <summary>
    /// Удалить запись измерений
    /// </summary>
    Task<bool> DeleteMeasurementsHistoryAsync(Guid id);
}
