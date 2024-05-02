﻿using Diary.Models.Entry;
using Diary.Models.Mood;
using Diary.Models.Pin;

namespace Diary.Clients.Interfaces;
public interface IEntryClient
{
    Task<ICollection<EntryListModel>> GetAllAsync(EntryFilter? entryFilter = null);

    Task<ICollection<EntryListModel>> GetByDayFromPreviousYears(DateTime date);

    Task<EntryDetailModel?> GetByIdAsync(Guid id);

    Task<Guid> SetAsync(EntryDetailModel model);

    Task DeleteAsync(EntryDetailModel model);

    Task<ICollection<MoodListModel>> GetMoodFromAllEntries();

    Task<ICollection<MoodListModel>> GetMoodFromEntriesByDateRange(DateTime dateFrom, DateTime dateTo);

    Task<ICollection<PinModel>> GetAllLocationPinsAsync();
}
