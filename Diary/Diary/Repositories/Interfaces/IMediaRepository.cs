using Diary.Entities;

namespace Diary.Repositories.Interfaces;

public interface IMediaRepository : IRepository<MediaEntity>
{
    Task<MediaEntity?> GetByFileNameAsync(string fileName);
    Task DeleteIfUnusedAsync(MediaEntity entity);
    Task DeleteIfUnusedAsync(ICollection<MediaEntity> entities);
    Task<bool> ExistsAsync(string fileName);
}
