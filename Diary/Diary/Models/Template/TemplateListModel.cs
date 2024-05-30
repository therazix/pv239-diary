namespace Diary.Models.Template;
public record TemplateListModel : ModelBase
{
    public required Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Mood { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool HasLocation => Latitude != null && Longitude != null;
}
