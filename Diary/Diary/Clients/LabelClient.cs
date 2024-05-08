using Diary.Clients.Interfaces;
using Diary.Mappers;
using Diary.Models.Label;
using Diary.Repositories.Interfaces;

namespace Diary.Clients;
public class LabelClient : ILabelClient
{
    private readonly ILabelRepository _repository;

    public LabelClient(ILabelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<LabelListModel>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        var ret = entities.MapToListModels();
        return ret;
    }

    public async Task<LabelDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    public async Task<LabelDetailModel> SetAsync(LabelDetailModel model)
    {
        var entity = model.MapToEntity();
        var savedEntity = await _repository.SetAsync(entity);

        return savedEntity.MapToDetailModel();
    }

    public async Task DeleteAsync(LabelDetailModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteAsync(entity);
    }
}
