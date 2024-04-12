using SQLite;

namespace Diary.Entities
{
    public class LabelEntity : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        [Ignore]
        public ICollection<EntryEntity> Entries { get; set; } = new List<EntryEntity>();

        [Ignore]
        public ICollection<TemplateEntity> Templates { get; set; } = new List<TemplateEntity>();
    }
}
