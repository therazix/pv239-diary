using Diary.Entities;
using Diary.Helpers;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class EntryRepository : RepositoryBase<EntryEntity>, IEntryRepository
{
    public EntryRepository() : base()
    {
    }

    public async override Task<ICollection<EntryEntity>> GetAllAsync()
    {
        var entities = await connection.Table<EntryEntity>().ToListAsync();
        foreach (var entity in entities)
        {
            await LinkRelatedEntities(entity);
        }

        return entities;
    }

    public async Task<ICollection<EntryEntity>> GetByDayFromPreviousYears(DateTime date)
    {
        var dayLastYear = date.AddYears(-1);

        var entities = await GetEntriesByDateRange(dayLastYear, dayLastYear);

        foreach (var entity in entities)
        {
            await LinkRelatedEntities(entity);
        }

        return entities;
    }

    public async Task<ICollection<EntryEntity>> GetByTimeMachineNotificationId(int timeMachineNotificationId)
    {
        var entities = await connection.Table<EntryEntity>()
            .Where(e => e.TimeMachineNotificationId == timeMachineNotificationId)
            .ToListAsync();

        foreach (var entity in entities)
        {
            await LinkRelatedEntities(entity);
        }

        return entities;
    }

    public async override Task<EntryEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<EntryEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntities(entity);
        }

        return entity;
    }

    public async new Task<EntryEntity> SetAsync(EntryEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now;
            entity.TimeMachineNotificationId = NotificationHelper.GetNotificationIdFromCreationDate(entity.CreatedAt);
        }
        entity.EditedAt = DateTime.Now;

        // Get existing labels that are connected to the entry
        var existingLabels = await GetLabelEntriesByEntryId(entity.Id);

        // Map label entities to 'link' entities
        var labelsToAdd = entity.Labels.Select(label => new LabelEntryEntity()
        {
            Id = Guid.NewGuid(),
            EntryId = entity.Id,
            LabelId = label.Id,
        });

        var labelEntriesToAdd = labelsToAdd.ExceptBy(existingLabels.Select(e => e.LabelId), e => e.LabelId).ToList();
        var labelEntriesToDelete = existingLabels.ExceptBy(labelsToAdd.Select(e => e.LabelId), e => e.LabelId).ToList();

        await connection.RunInTransactionAsync(tran =>
        {
            tran.InsertOrReplace(entity);
            tran.InsertAll(labelEntriesToAdd);
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }
        });

        return entity;
    }

    public async override Task DeleteAsync(EntryEntity entity)
    {
        var labelEntriesToDelete = await GetLabelEntriesByEntryId(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }
            tran.Delete(entity);
        });
    }

    public async Task<ICollection<EntryEntity>> GetEntriesByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);

        return await connection.Table<EntryEntity>()
            .Where(e => e.CreatedAt >= dateFrom && e.CreatedAt <= dateTo)
            .ToListAsync();
    }

    private async Task<ICollection<LabelEntryEntity>> GetLabelEntriesByEntryId(Guid id)
    {
        return await connection.Table<LabelEntryEntity>().Where(e => e.EntryId == id).ToListAsync();
    }

    private async Task LinkRelatedEntities(EntryEntity entity)
    {
        var labelIds = (await GetLabelEntriesByEntryId(entity.Id)).Select(e => e.LabelId).ToList();
        var labels = await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();

        entity.Labels = labels;
    }
}
