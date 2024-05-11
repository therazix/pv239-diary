using Diary.Models.Media;

namespace Diary.Clients.Interfaces;

public interface IMediaClient
{
    Task<ICollection<MediaModel>> GetAllAsync();
    Task<MediaModel?> GetByIdAsync(Guid id);
    Task<MediaModel> SetAsync(MediaModel model);
    Task<MediaModel> SetIfNewAsync(MediaModel model, bool byFilename = true);
    Task DeleteAsync(MediaModel model);
    Task DeleteIfUnusedAsync(MediaModel model, ICollection<Guid>? entriesToIgnore = null);
}
