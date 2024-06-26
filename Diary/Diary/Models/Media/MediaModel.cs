﻿using Diary.Enums;

namespace Diary.Models.Media;

public record MediaModel : ModelBase
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string OriginalFileName { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
