using Diary.Clients.Interfaces;
using Diary.Mappers;
using Diary.Models.Media;
using Diary.Repositories.Interfaces;
using Diary.Services;

namespace Diary.Clients;

public class MediaClient : IMediaClient
{
    private readonly IMediaRepository _repository;

    public MediaClient(IMediaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<MediaModel>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.MapToModels();
    }

    public async Task<MediaModel?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity?.MapToModel();
    }

    public async Task<MediaModel?> GetByFileNameAsync(string fileName)
    {
        var entity = await _repository.GetByFileNameAsync(fileName);
        return entity?.MapToModel();
    }

    /// <summary>
    /// Sets the media in the database. If the media already exists, it will be updated.
    /// If the media is linked to any entries, the links will be updated.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task<MediaModel> SetAsync(MediaModel model)
    {
        var entity = model.MapToEntity();
        var savedEntity = await _repository.SetAsync(entity);
        await MediaFileService.DeleteUnusedFilesAsync(_repository);
        return savedEntity.MapToModel();
    }

    /// <summary>
    /// Sets the media if it does not exist in the database. If <c>byFileName</c> is set to <c>true</c>,
    /// the filename will be used to check for existence. Otherwise, the ID will be used.
    /// If the media is linked to any entries, the links will be updated.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task<MediaModel> SetIfNewAsync(MediaModel model, bool byFilename = true)
    {
        var existing = byFilename ? await _repository.GetByFileNameAsync(model.FileName) : await _repository.GetByIdAsync(model.Id);
        if (existing != null)
        {
            return existing.MapToModel();
        }

        return await SetAsync(model);
    }

    /// <summary>
    /// Deletes the media from database. If the media is linked to any entries, the link will be removed.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task DeleteAsync(MediaModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteAsync(entity);
        await MediaFileService.DeleteUnusedFilesAsync(_repository);
    }

    /// <summary>
    /// Deletes the media from database if it is not linked to any entries.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task DeleteIfUnusedAsync(MediaModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteIfUnusedAsync(entity);
        await MediaFileService.DeleteUnusedFilesAsync(_repository);
    }

    /// <summary>
    /// Deletes the media from database if they are not linked to any entries.
    /// Media files are also updated to reflect the changes.
    /// </summary>
    public async Task DeleteIfUnusedAsync(ICollection<MediaModel> models)
    {
        var entities = models.MapToEntites();
        await _repository.DeleteIfUnusedAsync(entities);
        await MediaFileService.DeleteUnusedFilesAsync(_repository);
    }
}
