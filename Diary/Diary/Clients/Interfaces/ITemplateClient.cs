using Diary.Models.Template;

namespace Diary.Clients.Interfaces;
public interface ITemplateClient
{
    Task<ICollection<TemplateListModel>> GetAllAsync();

    Task<ICollection<TemplateDetailModel>> GetAllDetailedAsync();

    Task<TemplateDetailModel?> GetByIdAsync(Guid id);

    Task<TemplateDetailModel> SetAsync(TemplateDetailModel model);

    Task DeleteAsync(TemplateDetailModel model);
}
