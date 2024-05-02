using Diary.Entities;

namespace Diary.Repositories.Interfaces;

public interface IMediaRepository : IRepository<MediaEntity>
{
    Task<string> SaveFileAsync(FileResult fileResult);
    void DeleteFile(string fileName);
    Task DeleteUnusedMediaAsync();
}
