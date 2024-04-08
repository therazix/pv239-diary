using Diary.Models.Entry;
using System.Collections.ObjectModel;

namespace Diary.Models.Label;
public record LabelDetailModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public ObservableCollection<EntryListModel> Entries { get; set; } = new();
}
