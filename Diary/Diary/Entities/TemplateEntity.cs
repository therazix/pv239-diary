using SQLite;

namespace Diary.Entities
{
    public class TemplateEntity : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int Mood { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Ignore]
        public ICollection<LabelEntity> Labels { get; set; } = new List<LabelEntity>();
    }
}
