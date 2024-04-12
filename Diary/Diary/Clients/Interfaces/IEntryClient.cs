using Diary.Models.Entry;

namespace Diary.Clients.Interfaces;
public interface IEntryClient
{
    Task<ICollection<EntryListModel>> GetAllAsync();

    Task<EntryDetailModel?> GetByIdAsync(Guid id);

    Task<Guid> SetAsync(EntryDetailModel model);

    Task DeleteAsync(EntryDetailModel model);
}
