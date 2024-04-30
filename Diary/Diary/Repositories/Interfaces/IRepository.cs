using Diary.Entities;
using SQLite;

namespace Diary.Repositories.Interfaces;
public interface IRepository<T> where T : EntityBase, new()
{
    Task<CreateTableResult> CreateTableAsync();

    Task<ICollection<T>> GetAllAsync();

    Task<ICollection<T>> GetAllAsync(ICollection<Guid> ids);

    Task<T?> GetByIdAsync(Guid id);

    Task<Guid> SetAsync(T entity);

    Task DeleteAsync(T entity);

    Task DeleteAsync(ICollection<T> entities);

    Task<bool> ExistsAsync(Guid id);
}
