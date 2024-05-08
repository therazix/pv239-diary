using Diary.Models.Label;

namespace Diary.Clients.Interfaces;
public interface ILabelClient
{
    Task<ICollection<LabelListModel>> GetAllAsync();

    Task<LabelDetailModel?> GetByIdAsync(Guid id);

    Task<LabelDetailModel> SetAsync(LabelDetailModel model);

    Task DeleteAsync(LabelDetailModel model);
}
