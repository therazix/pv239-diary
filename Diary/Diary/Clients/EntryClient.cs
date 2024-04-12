using Diary.Clients.Interfaces;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Repositories.Interfaces;

namespace Diary.Clients;
public class EntryClient : IEntryClient
{
    private readonly IEntryRepository _repository;

    public EntryClient(IEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<EntryListModel>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.MapToListModels();
    }

    public async Task<EntryDetailModel?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity?.MapToDetailModel();
    }

    public async Task<Guid> SetAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();
        return await _repository.SetAsync(entity);
    }

    public async Task DeleteAsync(EntryDetailModel model)
    {
        var entity = model.MapToEntity();
        await _repository.DeleteAsync(entity);
    }
}
