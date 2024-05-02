using Diary.Enums;

namespace Diary.Models.Media;

public record MediaModel : ModelBase
{
    public string FileName { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
}
