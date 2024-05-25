using SQLite;

namespace Diary.Entities
{
    public class MediaEntity : EntityBase
    {
        public string FileName { get; set; } = null!;
        public string OriginalFileName { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        [Ignore]
        public ICollection<EntryEntity> Entries { get; set; } = new List<EntryEntity>();
    }
}
