using Diary.Entities;

namespace Diary.Repositories.Interfaces;

public interface IMediaRepository : IRepository<MediaEntity>
{
    Task<MediaEntity?> GetByFileNameAsync(string fileName);
    Task DeleteIfUnusedAsync(MediaEntity entity, ICollection<Guid>? entriesToIgnore = null);
    Task DeleteIfUnusedAsync(ICollection<MediaEntity> entities, ICollection<Guid>? entriesToIgnore = null);
    Task<bool> ExistsAsync(string fileName);
}
