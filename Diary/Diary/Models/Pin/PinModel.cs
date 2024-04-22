namespace Diary.Models.Pin;

public record PinModel : ModelBase
{
    public Guid EntryId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Location Location { get; set; } = null!;
}
