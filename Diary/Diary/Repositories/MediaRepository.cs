using Diary.Entities;
using Diary.Repositories.Interfaces;
using System.Security.Cryptography;

namespace Diary.Repositories;

public class MediaRepository : RepositoryBase<MediaEntity>, IMediaRepository
{
    public MediaRepository() : base()
    {
    }

    public async Task<string> SaveFileAsync(FileResult fileResult)
    {
        using Stream sourceStream = await fileResult.OpenReadAsync();
        var fileHash = CalculateMD5(sourceStream);
        sourceStream.Position = 0;

        var fileName = fileHash + Path.GetExtension(fileResult.FileName);
        var targetFilePath = Path.Combine(Constants.MediaPath, fileName);

        if (File.Exists(targetFilePath))
        {
            return fileName;
        }

        using FileStream targetStream = File.OpenWrite(targetFilePath);
        await sourceStream.CopyToAsync(targetStream);

        return fileName;
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

    private static string CalculateMD5(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
