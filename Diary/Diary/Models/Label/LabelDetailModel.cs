using Diary.Models.Entry;
using Diary.Models.Template;
using System.Collections.ObjectModel;

namespace Diary.Models.Label;
public record LabelDetailModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public ObservableCollection<EntryListModel> Entries { get; set; } = new();
    public ObservableCollection<TemplateListModel> Templates { get; set; } = new();
}
