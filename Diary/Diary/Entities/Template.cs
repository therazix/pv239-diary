namespace Diary.Entities
{
    public class Template : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public virtual ICollection<Label> Labels { get; set; } = [];
    }
}
