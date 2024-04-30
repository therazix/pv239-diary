namespace Diary.Models.Entry;
public record EntryListModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public bool IsFavorite { get; set; }
}
