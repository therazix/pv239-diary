using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class TemplateRepository : RepositoryBase<TemplateEntity>, ITemplateRepository
{
    public TemplateRepository() : base()
    {
    }

    public override async Task<ICollection<TemplateEntity>> GetAllAsync()
    {
        var entities = await connection.Table<TemplateEntity>().ToListAsync();
        foreach (var entity in entities)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entities;
    }

    public override async Task<TemplateEntity?> GetByIdAsync(Guid id)
    {
        var entity = await connection.Table<TemplateEntity>().Where(e => e.Id == id).FirstOrDefaultAsync();
        if (entity != null)
        {
            await LinkRelatedEntitiesAsync(entity);
        }

        return entity;
    }

    public override async Task<TemplateEntity> SetAsync(TemplateEntity entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        // Get existing labels that are connected to the template
        var existingLabels = await GetLabelTemplatesByTemplateIdAsync(entity.Id);

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

        return entity;
    }

    public override async Task DeleteAsync(TemplateEntity entity)
    {
        var labelTemplatesToDelete = await GetLabelTemplatesByTemplateIdAsync(entity.Id);
        await connection.RunInTransactionAsync(tran =>
        {
            foreach (var labelTemplateToDelete in labelTemplatesToDelete)
            {
                tran.Delete(labelTemplateToDelete);
            }
            tran.Delete(entity);
        });
    }

    private async Task<ICollection<LabelTemplateEntity>> GetLabelTemplatesByTemplateIdAsync(Guid id)
    {
        return await connection.Table<LabelTemplateEntity>().Where(e => e.TemplateId == id).ToListAsync();
    }

    private async Task LinkRelatedEntitiesAsync(TemplateEntity entity)
    {
        var labelIds = (await GetLabelTemplatesByTemplateIdAsync(entity.Id)).Select(e => e.LabelId).ToList();
        var labels = await connection.Table<LabelEntity>().Where(e => labelIds.Contains(e.Id)).ToListAsync();

        entity.Labels = labels;
    }
}
