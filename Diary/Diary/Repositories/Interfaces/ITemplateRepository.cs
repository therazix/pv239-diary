using Diary.Entities;

namespace Diary.Repositories.Interfaces;
public interface ITemplateRepository : IRepository<TemplateEntity>
{
    Task<ICollection<TemplateEntity>> GetAllDetailedAsync();
}
