namespace Diary.Entities
{
    public class Label : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string Colour { get; set; } = string.Empty;

        public virtual ICollection<Entry> Entries { get; set; } = [];
    }
}
