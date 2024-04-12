using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;
public class LabelTemplateRepository : RepositoryBase<LabelTemplateEntity>, ILabelTemplateRepository
{
    public LabelTemplateRepository() : base()
    {
    }
}
