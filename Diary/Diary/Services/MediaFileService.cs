using Diary.Repositories.Interfaces;
using System.Security.Cryptography;

namespace Diary.Services;
public static class MediaFileService
{
    /// <summary>
    /// Saves the file to the media folder and returns the file name.
    /// If the file already exists, it will not be overwritten and the
    /// existing file name will be returned. This method does not modify
    /// the database in any way.
    /// </summary>
    public static async Task<string> SaveAsync(FileResult fileResult)
    {
        using Stream sourceStream = await fileResult.OpenReadAsync();
        var fileName = fileResult.FileName;

        return await SaveAsync(sourceStream, fileName);
    }

    /// <summary>
    /// Saves the file to the media folder and returns the file name.
    /// If the file already exists, it will not be overwritten and the
    /// existing file name will be returned. This method does not modify
    /// the database in any way.
    /// </summary>
    public static async Task<string> SaveAsync(Stream sourceStream, string fileName)
    {
        var fileHash = CalculateMD5(sourceStream);
        sourceStream.Position = 0;

        var newFileName = fileHash + Path.GetExtension(fileName);
        var targetFilePath = Path.Combine(Constants.MediaPath, newFileName);

        if (File.Exists(targetFilePath))
        {
            return newFileName;
        }

        using FileStream targetStream = File.OpenWrite(targetFilePath);
        await sourceStream.CopyToAsync(targetStream);

        return newFileName;
    }

    /// <summary>
    /// Deletes the file specified by the <c>fileName</c> from the media folder.
    /// This method does not modify the database in any way.
    /// </summary>
    public static void Delete(string fileName)
    {
        string filePath = Path.Combine(Constants.MediaPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Deletes all the files specified by the <c>fileNames</c> from the media folder.
    /// This method does not modify the database in any way.
    /// </summary>
    public static void Delete(ICollection<string> fileNames)
    {
        foreach (string filePath in Directory.GetFiles(Constants.MediaPath))
        {
            if (fileNames.Contains(Path.GetFileName(filePath)))
            {
                File.Delete(filePath);
            }
        }
    }

    /// <summary>
    /// Deletes all media files that are not referenced by any media entity.
    /// This method does not modify the database in any way.
    /// </summary>
    public static async Task DeleteUnusedFilesAsync(IMediaRepository mediaRepository)
    {
        var media = await mediaRepository.GetAllAsync();
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
