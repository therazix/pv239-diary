using System.Collections.ObjectModel;

namespace Diary.Models.Media;

public record MediaGroupModel : ModelBase
{
    public DateTime? Date { get; set; }
    public ObservableCollection<MediaModel> Media { get; set; } = new();
}
