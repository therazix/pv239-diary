﻿using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class LabelRepository : RepositoryBase<LabelEntity>, ILabelRepository
{
    public LabelRepository() : base()
    {
    }

    public async override Task<LabelEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<LabelEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            var entryIds = (await GetLabelEntriesByLabelId(id)).Select(e => e.EntryId).ToList();
            var templateIds = (await GetLabelTemplatesByLabelId(id)).Select(e => e.TemplateId).ToList();

            var entries = await connection.Table<EntryEntity>().Where(e => entryIds.Contains(e.Id)).ToListAsync();
            var templates = await connection.Table<TemplateEntity>().Where(e => templateIds.Contains(e.Id)).ToListAsync();

            entity.Entries = entries;
            entity.Templates = templates;
        }

        return entity;
    }

    public async override Task<Guid> SetAsync(LabelEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        // Get existing entries and templates that are connected to the label
        var existingEntries = await GetLabelEntriesByLabelId(entity.Id);
        var existingTemplates = await GetLabelTemplatesByLabelId(entity.Id);

        // Map entry and template entities to 'link' entities
        var entriesToAdd = entity.Entries.Select(entry => new LabelEntryEntity()
        {
            Id = Guid.NewGuid(),
            EntryId = entry.Id,
            LabelId = entity.Id,
        });
        var templatesToAdd = entity.Templates.Select(template => new LabelTemplateEntity()
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            LabelId = entity.Id,
        });


        var labelEntriesToAdd = entriesToAdd.ExceptBy(existingEntries.Select(e => e.EntryId), e => e.EntryId).ToList();
        var labelEntriesToDelete = existingEntries.ExceptBy(entriesToAdd.Select(e => e.EntryId), e => e.EntryId).ToList();

        var labelTemplatesToAdd = templatesToAdd.ExceptBy(existingTemplates.Select(e => e.TemplateId), e => e.TemplateId).ToList();
        var labelTemplatesToDelete = existingTemplates.ExceptBy(templatesToAdd.Select(e => e.TemplateId), e => e.TemplateId).ToList();

        await connection.RunInTransactionAsync(tran =>
        {
            tran.InsertOrReplace(entity);

            tran.InsertAll(labelEntriesToAdd);
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }

            tran.InsertAll(labelTemplatesToAdd);
            foreach (var labelTemplateToDelete in labelTemplatesToDelete)
            {
                tran.Delete(labelTemplateToDelete);
            }
        });

        return entity.Id;
    }

    public async override Task DeleteAsync(LabelEntity entity)
    {
        var labelEntriesToDelete = await GetLabelEntriesByLabelId(entity.Id);
        var labelTemplatesToDelete = await GetLabelTemplatesByLabelId(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var labelEntryToDelete in labelEntriesToDelete)
            {
                tran.Delete(labelEntryToDelete);
            }
            foreach (var labelTemplateToDelete in labelTemplatesToDelete)
            {
                tran.Delete(labelTemplateToDelete);
            }
            tran.Delete(entity);
        });
    }

    private async Task<ICollection<LabelEntryEntity>> GetLabelEntriesByLabelId(Guid id)
    {
        return await connection.Table<LabelEntryEntity>().Where(e => e.LabelId == id).ToListAsync();
    }

    private async Task<ICollection<LabelTemplateEntity>> GetLabelTemplatesByLabelId(Guid id)
    {
        return await connection.Table<LabelTemplateEntity>().Where(e => e.LabelId == id).ToListAsync();
    }
}
