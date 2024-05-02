namespace Diary.Clients.Interfaces;

public interface IMediaClient
{
    Task<string> SaveFileAsync(FileResult fileResult);
    void DeleteFile(string fileName);
    Task DeleteUnusedMediaAsync();
}
