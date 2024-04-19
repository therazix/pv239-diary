using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class TemplateRepository : RepositoryBase<TemplateEntity>, ITemplateRepository
{
    public TemplateRepository() : base()
    {
    }

    public async Task<ICollection<TemplateEntity>> GetAllDetailedAsync()
    {
        var entities = await connection.Table<TemplateEntity>().ToListAsync();

        foreach (var entity in entities)
        {
            var labels = await GetLabelsByTemplateId(entity.Id);

            entity.Labels = labels;
        }

        return entities;
    }

    public async override Task<TemplateEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<TemplateEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            var labelIds = (await GetLabelTemplatesByTemplateId(id)).Select(e => e.LabelId).ToList();
            var labels = await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();

            entity.Labels = labels;
        }

        return entity;
    }

    public async override Task<Guid> SetAsync(TemplateEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        // Get existing labels that are connected to the template
        var existingLabels = await GetLabelTemplatesByTemplateId(entity.Id);

        // Map label entities to 'link' entities
        var labelsToAdd = entity.Labels.Select(label => new LabelTemplateEntity()
        {
            Id = Guid.NewGuid(),
            TemplateId = entity.Id,
            LabelId = label.Id,
        });

        var labelTemplatesToAdd = labelsToAdd.ExceptBy(existingLabels.Select(e => e.LabelId), e => e.LabelId).ToList();
        var labelTemplatesToDelete = existingLabels.ExceptBy(labelsToAdd.Select(e => e.LabelId), e => e.LabelId).ToList();

        await connection.RunInTransactionAsync(tran =>
        {
            tran.InsertOrReplace(entity);
            tran.InsertAll(labelTemplatesToAdd);
            foreach (var labelTemplateToDelete in labelTemplatesToDelete)
            {
                tran.Delete(labelTemplateToDelete);
            }
        });

        return entity.Id;
    }

    public async override Task DeleteAsync(TemplateEntity entity)
    {
        var labelTemplatesToDelete = await GetLabelTemplatesByTemplateId(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var labelTemplateToDelete in labelTemplatesToDelete)
            {
                tran.Delete(labelTemplateToDelete);
            }
            tran.Delete(entity);
        });
    }

    private async Task<ICollection<LabelTemplateEntity>> GetLabelTemplatesByTemplateId(Guid id)
    {
        return await connection.Table<LabelTemplateEntity>().Where(e => e.TemplateId == id).ToListAsync();
    }

    private async Task<ICollection<LabelEntity>> GetLabelsByTemplateId(Guid id)
    {
        var labelIds = (await GetLabelTemplatesByTemplateId(id)).Select(e => e.LabelId).ToList();
        return await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();
    }
}
