namespace Diary.Models.Label;
public record LabelListModel : ModelBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Color Color { get; set; } = Constants.DefaultLabelColor;
}
