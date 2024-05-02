using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;

public class MediaRepository : RepositoryBase<MediaEntity>, IMediaRepository
{
    public MediaRepository() : base()
    {
    }

    public async Task<string> SaveFileAsync(FileResult fileResult)
    {
        string targetFilePath = Path.Combine(Constants.MediaPath, fileResult.FileName);
        using FileStream targetStream = File.OpenWrite(targetFilePath);
        using Stream sourceStream = await fileResult.OpenReadAsync();

        await sourceStream.CopyToAsync(targetStream);

        return fileResult.FileName;
    }

    public void DeleteFile(string fileName)
    {
        string filePath = Path.Combine(Constants.MediaPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task DeleteUnusedMediaAsync()
    {
        var media = await connection.Table<MediaEntity>().ToListAsync();
        var usedNames = media.Select(m => m.FileName).ToList();

        foreach (string filePath in Directory.GetFiles(Constants.MediaPath))
        {
            if (!usedNames.Contains(Path.GetFileName(filePath)))
            {
                File.Delete(filePath);
            }
        }
    }
}
