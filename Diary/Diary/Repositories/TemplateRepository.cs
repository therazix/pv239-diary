using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class TemplateRepository : RepositoryBase<TemplateEntity>, ITemplateRepository
{
    public TemplateRepository() : base()
    {
    }

    public async override Task<ICollection<TemplateEntity>> GetAllAsync()
    {
        var entities = await connection.Table<TemplateEntity>().ToListAsync();
        foreach (var entity in entities)
        {
            await LinkRelatedEntities(entity);
        }

        return entities;
    }

    public async override Task<TemplateEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<TemplateEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntities(entity);
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

    private async Task LinkRelatedEntities(TemplateEntity entity)
    {
        var labelIds = (await GetLabelTemplatesByTemplateId(entity.Id)).Select(e => e.LabelId).ToList();
        var labels = await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();

        entity.Labels = labels;
    }
}
