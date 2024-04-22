using Diary.Entities;
using SQLite;

namespace Diary.Repositories;
public class RepositoryBase<T> where T : EntityBase, new()
{
    protected SQLiteAsyncConnection connection;

    public RepositoryBase()
    {
        connection = new SQLiteAsyncConnection(Constants.DatabasePath);
    }

    public virtual async Task<CreateTableResult> CreateTableAsync()
    {
        return await connection.CreateTableAsync<T>();
    }

    public virtual async Task<ICollection<T>> GetAllAsync()
    {
        return await connection.Table<T>().ToListAsync();
    }

    public virtual async Task<ICollection<T>> GetAllAsync(ICollection<Guid> ids)
    {
        return await connection.Table<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await connection.Table<T>().Where(e => e.Id == id).FirstOrDefaultAsync();
    }

    public virtual async Task<Guid> SetAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        if (await ExistsAsync(entity.Id))
        {
            await connection.UpdateAsync(entity);
        }
        else
        {
            await connection.InsertAsync(entity);
        }

        return entity.Id;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        await connection.DeleteAsync(entity);
    }

    public virtual async Task DeleteAsync(ICollection<T> entities)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await connection.Table<T>().Where(e => e.Id == id).FirstOrDefaultAsync();
        return entity != null;
    }
}
