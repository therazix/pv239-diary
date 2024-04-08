using Diary.Models.Label;
using System.Collections.ObjectModel;

namespace Diary.Models.Template;
public record TemplateDetailModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Mood { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public ObservableCollection<LabelListModel> Labels { get; set; } = new();
}
