using Diary.Models.Entry;
using Diary.Models.Mood;

namespace Diary.Clients.Interfaces;
public interface IEntryClient
{
    Task<ICollection<EntryListModel>> GetAllAsync();

    Task<EntryDetailModel?> GetByIdAsync(Guid id);

    Task<Guid> SetAsync(EntryDetailModel model);

    Task DeleteAsync(EntryDetailModel model);

    Task<ICollection<MoodListModel>> GetMoodFromAllEntries();

    Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRange(DateTime dateFrom, DateTime dateTo);
}
