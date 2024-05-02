using Diary.Entities;

namespace Diary.Repositories.Interfaces;
public interface IEntryRepository : IRepository<EntryEntity>
{
    Task<ICollection<EntryEntity>> GetEntriesByDateRange(DateTime dateFrom, DateTime dateTo);

    Task<ICollection<EntryEntity>> GetByDayFromPreviousYears(DateTime date);

    Task<ICollection<EntryEntity>> GetByTimeMachineNotificationId(int timeMachineNotificationId);
    
    new Task<EntryEntity> SetAsync(EntryEntity entity);
}
