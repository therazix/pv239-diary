using Diary.Entities;

namespace Diary.Repositories.Interfaces;
public interface IEntryRepository : IRepository<EntryEntity>
{
    Task<ICollection<EntryEntity>> GetEntriesByDateRange(DateTime dateFrom, DateTime dateTo);
}
