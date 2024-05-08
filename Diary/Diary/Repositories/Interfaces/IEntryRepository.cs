using Diary.Entities;

namespace Diary.Repositories.Interfaces;
public interface IEntryRepository : IRepository<EntryEntity>
{
    Task<ICollection<EntryEntity>> GetEntriesByDateRangeAsync(DateTime dateFrom, DateTime dateTo);

    Task<ICollection<EntryEntity>> GetByDayFromPreviousYearsAsync(DateTime date);

    Task<ICollection<EntryEntity>> GetByTimeMachineNotificationIdAsync(int timeMachineNotificationId);
}
