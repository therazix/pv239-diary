using Diary.Models.Label;
using System.Collections.ObjectModel;

namespace Diary.Models.Entry;
public record EntryDetailModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public bool IsFavorite { get; set; }
    public int Mood { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public ObservableCollection<LabelListModel> Labels { get; set; } = new();
}
