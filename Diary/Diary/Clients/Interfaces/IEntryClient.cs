using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;

namespace Diary.Clients.Interfaces;
public interface IEntryClient
{
    Task<ICollection<EntryListModel>> GetAllAsync(EntryFilterModel? entryFilter = null);

    Task<ICollection<EntryListModel>> GetByDayFromPreviousYearsAsync(DateTime date);

    Task<EntryDetailModel?> GetByIdAsync(Guid id);

    Task<EntryDetailModel> SetAsync(EntryDetailModel model);

    Task DeleteAsync(EntryDetailModel model);

    Task<ICollection<MoodListModel>> GetMoodFromAllEntriesAsync();

    Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRangeAsync(DateTime dateFrom, DateTime dateTo);

    Task<ICollection<PinModel>> GetAllLocationPinsAsync();
}
