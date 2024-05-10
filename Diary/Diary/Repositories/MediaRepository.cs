using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;

public class MediaRepository : RepositoryBase<MediaEntity>, IMediaRepository
{
    public MediaRepository() : base()
    {
    }

    public override async Task<ICollection<MediaEntity>> GetAllAsync()
    {
        var entities = await connection.Table<MediaEntity>().ToListAsync();
        foreach (var entity in entities)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entities;
    }

    public override async Task<MediaEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<MediaEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entity;
    }

    public async Task<MediaEntity?> GetByFileNameAsync(string fileName)
    {
        var entity = await connection.Table<MediaEntity>().Where(e => e.FileName == fileName).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entity;
    }

    public override async Task<MediaEntity> SetAsync(MediaEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now;
        }

        var existingEntries = await GetEntryMediaByMediaIdAsync(entity.Id);

        var entriesToAdd = entity.Entries.Select(entry => new EntryMediaEntity()
        {
            Id = Guid.NewGuid(),
            EntryId = entry.Id,
            MediaId = entity.Id,
        });

        var entryMediaToAdd = entriesToAdd.ExceptBy(existingEntries.Select(e => e.EntryId), e => e.EntryId).ToList();
        var entryMediaToDelete = existingEntries.ExceptBy(entriesToAdd.Select(e => e.EntryId), e => e.EntryId).ToList();

        await connection.RunInTransactionAsync(tran =>
        {
            tran.InsertOrReplace(entity);
            tran.InsertAll(entryMediaToAdd);
            foreach (var entryMedia in entryMediaToDelete)
            {
                tran.Delete(entryMedia);
            }
        });

        return entity;
    }

    /// <summary>
    /// Deletes the media entity. If the media entity is linked to any entries, the link will be removed.
    /// </summary>
    public override async Task DeleteAsync(MediaEntity entity)
    {
        var entryMediaToDelete = await GetEntryMediaByMediaIdAsync(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var entryMedia in entryMediaToDelete)
            {
                tran.Delete(entryMedia);
            }
            tran.Delete(entity);
        });
    }

    /// <summary>
    /// Deletes the media entity if it is not linked to any entries.
    /// </summary>
    public async Task DeleteIfUnusedAsync(MediaEntity entity)
    {
        var entryMediaToDelete = await GetEntryMediaByMediaIdAsync(entity.Id);
        if (entryMediaToDelete.Count == 0)
        {
            await connection.DeleteAsync(entity);
        }
    }

    /// <summary>
    /// Deletes the media entities if they are not linked to any entries.
    /// </summary>
    public async Task DeleteIfUnusedAsync(ICollection<MediaEntity> entities)
    {
        foreach (var entity in entities)
        {
            await DeleteIfUnusedAsync(entity);
        }
    }

    public async Task<bool> ExistsAsync(string fileName)
    {
        var entity = await connection.Table<MediaEntity>().Where(e => e.FileName == fileName).FirstOrDefaultAsync();
        return entity != null;
    }


    private async Task<ICollection<EntryMediaEntity>> GetEntryMediaByMediaIdAsync(Guid id)
    {
        return await connection.Table<EntryMediaEntity>().Where(e => e.MediaId == id).ToListAsync();
    }

    private async Task LinkRelatedEntitiesAsync(MediaEntity entity)
    {
        var entryIds = (await GetEntryMediaByMediaIdAsync(entity.Id)).Select(e => e.EntryId).ToList();
        entity.Entries = await connection.Table<EntryEntity>().Where(e => entryIds.Contains(e.Id)).ToListAsync();
    }
}
