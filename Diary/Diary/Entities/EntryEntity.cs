using SQLite;

namespace Diary.Entities
{
    public class EntryEntity : EntityBase
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime EditedAt { get; set; }

        public bool IsFavorite { get; set; }

        public int Mood { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int NotificationId { get; set; }

        [Ignore]
        public ICollection<LabelEntity> Labels { get; set; } = new List<LabelEntity>();

        [Ignore]
        public ICollection<MediaEntity> Media { get; set; } = new List<MediaEntity>();
    }
}
