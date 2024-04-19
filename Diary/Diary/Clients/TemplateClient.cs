using Diary.Clients.Interfaces;
using Diary.Mappers;
using Diary.Models.Template;
using Diary.Repositories.Interfaces;

namespace Diary.Clients;
public class TemplateClient : ITemplateClient
{
    private readonly ITemplateRepository _repository;

    public TemplateClient(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<TemplateListModel>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.MapToListModels();
    }

    public async Task<ICollection<TemplateDetailModel>> GetAllDetailedAsync()
    {
        var entities = await _repository.GetAllDetailedAsync();
        return entities.MapToDetailModels();
    }

    public async Task<TemplateDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    public async Task<Guid> SetAsync(TemplateDetailModel model)
    {
        var entity = model.MapToEntity();
        return await _repository.SetAsync(entity);
    }

    public async Task DeleteAsync(TemplateDetailModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteAsync(entity);
    }
}
