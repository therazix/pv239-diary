using Diary.Clients.Interfaces;
using Diary.Repositories.Interfaces;

namespace Diary.Clients;

public class MediaClient : IMediaClient
{
    private readonly IMediaRepository _repository;

    public MediaClient(IMediaRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> SaveFileAsync(FileResult fileResult)
    {
        return await _repository.SaveFileAsync(fileResult);
    }

    public void DeleteFile(string fileName)
    {
        _repository.DeleteFile(fileName);
    }

    public async Task DeleteUnusedMediaAsync()
    {
        await _repository.DeleteUnusedMediaAsync();
    }
}
