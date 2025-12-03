using System.Linq.Expressions;

namespace SetRep.Core.Repositories;

/// <summary>
/// Базовый интерфейс репозитория
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Получить все сущности
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Получить сущность по идентификатору
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Найти сущности по условию
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Получить первую сущность по условию
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Добавить сущность
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Добавить несколько сущностей
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Обновить сущность
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Удалить сущность
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Удалить сущность по идентификатору
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Проверить существование сущности
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    Task<int> SaveChangesAsync();
}

