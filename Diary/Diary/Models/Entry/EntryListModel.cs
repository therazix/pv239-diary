namespace Diary.Models.Entry;
public record EntryListModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public bool IsFavorite { get; set; }
    public int MediaCount { get; init; }
}
