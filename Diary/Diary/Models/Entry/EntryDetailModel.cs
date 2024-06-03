using Diary.Models.Label;
using Diary.Models.Media;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Diary.Models.Entry;
public record EntryDetailModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public bool IsFavorite { get; set; }

    [Range(1, 5)]
    public int Mood { get; set; } = 1;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int NotificationId { get; set; }
    public ObservableCollection<LabelListModel> Labels { get; set; } = new();
    public ObservableCollection<MediaModel> Media { get; set; } = new();
}
