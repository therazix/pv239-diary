namespace Diary.Entities;
public class LabelEntryEntity : EntityBase
{
    public Guid LabelId { get; set; }
    public Guid EntryId { get; set; }
}
