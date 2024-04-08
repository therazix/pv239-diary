using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class LabelEntryRepository : RepositoryBase<LabelEntryEntity>, ILabelEntryRepository
{
    public LabelEntryRepository() : base()
    {
    }
}
