namespace Diary.Entities
{
    public class MediaEntity : EntityBase
    {
        public Guid EntryId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
    }
}
