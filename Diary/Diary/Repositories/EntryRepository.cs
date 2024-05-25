using Diary.Entities;
using Diary.Helpers;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class EntryRepository : RepositoryBase<EntryEntity>, IEntryRepository
{
    public EntryRepository() : base()
    {
    }

    public override async Task<ICollection<EntryEntity>> GetAllAsync()
    {
        var entities = await connection.Table<EntryEntity>().ToListAsync();
        foreach (var entity in entities)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entities;
    }

    public async Task<ICollection<EntryEntity>> GetByDayFromPreviousYearsAsync(DateTime date)
    {
        var entities = await GetAllAsync();

        entities = entities
            .Where(e => e.CreatedAt.Month == date.Month && e.CreatedAt.Day == date.Day && e.CreatedAt.Year < date.Year)
            .ToList();

        return entities;
    }

    public async Task<ICollection<EntryEntity>> GetByNotificationIdAsync(int notificationId)
    {
        var entities = await connection.Table<EntryEntity>()
            .Where(e => e.NotificationId == notificationId)
            .ToListAsync();

        foreach (var entity in entities)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entities;
    }

    public override async Task<EntryEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<EntryEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entity;
    }

    public override async Task<EntryEntity> SetAsync(EntryEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now;
            entity.NotificationId = NotificationHelper.GetNotificationIdFromCreationDate(entity.CreatedAt);
        }
        entity.EditedAt = DateTime.Now;

        // Get existing labels and media that are connected to the entry
        var existingLabels = await GetLabelEntriesByEntryIdAsync(entity.Id);
        var existingMedia = await GetEntryMediaByEntryIdAsync(entity.Id);

        // Map label entities to 'link' entities
        var labelsToAdd = entity.Labels.Select(label => new LabelEntryEntity()
        {
            Id = Guid.NewGuid(),
            EntryId = entity.Id,
            LabelId = label.Id,
        });

        var mediaToAdd = entity.Media.Select(media => new EntryMediaEntity()
        {
            Id = Guid.NewGuid(),
            EntryId = entity.Id,
            MediaId = media.Id,
        });

        var labelEntriesToAdd = labelsToAdd.ExceptBy(existingLabels.Select(e => e.LabelId), e => e.LabelId).ToList();
        var labelEntriesToDelete = existingLabels.ExceptBy(labelsToAdd.Select(e => e.LabelId), e => e.LabelId).ToList();

        var entryMediaToAdd = mediaToAdd.ExceptBy(existingMedia.Select(e => e.MediaId), e => e.MediaId).ToList();
        var entryMediaToDelete = existingMedia.ExceptBy(mediaToAdd.Select(e => e.MediaId), e => e.MediaId).ToList();

        await connection.RunInTransactionAsync(tran =>
        {
            tran.InsertOrReplace(entity);
            tran.InsertAll(labelEntriesToAdd);
            tran.InsertAll(entryMediaToAdd);
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }
            foreach (var entryMedia in entryMediaToDelete)
            {
                tran.Delete(entryMedia);
            }
        });

        return entity;
    }

    public override async Task DeleteAsync(EntryEntity entity)
    {
        var labelEntriesToDelete = await GetLabelEntriesByEntryIdAsync(entity.Id);
        var entryMediaToDelete = await GetEntryMediaByEntryIdAsync(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }
            foreach (var entryMedia in entryMediaToDelete)
            {
                tran.Delete(entryMedia);
            }
            tran.Delete(entity);
        });
    }

    public async Task<ICollection<EntryEntity>> GetEntriesByDateRangeAsync(DateTime dateFrom, DateTime dateTo)
    {
        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);

        return await connection.Table<EntryEntity>()
            .Where(e => e.CreatedAt >= dateFrom && e.CreatedAt <= dateTo)
            .ToListAsync();
    }

    private async Task<ICollection<LabelEntryEntity>> GetLabelEntriesByEntryIdAsync(Guid id)
    {
        return await connection.Table<LabelEntryEntity>().Where(e => e.EntryId == id).ToListAsync();
    }

    private async Task<ICollection<EntryMediaEntity>> GetEntryMediaByEntryIdAsync(Guid id)
    {
        return await connection.Table<EntryMediaEntity>().Where(e => e.EntryId == id).ToListAsync();
    }

    private async Task LinkRelatedEntitiesAsync(EntryEntity entity)
    {
        var labelIds = (await GetLabelEntriesByEntryIdAsync(entity.Id)).Select(e => e.LabelId).ToList();
        var mediaIds = (await GetEntryMediaByEntryIdAsync(entity.Id)).Select(e => e.MediaId).ToList();
        entity.Labels = await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();
        entity.Media = await connection.Table<MediaEntity>().Where(e => mediaIds.Contains(e.Id)).ToListAsync();
    }
}
