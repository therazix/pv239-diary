namespace Diary.Entities;
public class ImportExportEntity
{
    public ICollection<EntryEntity> Entries { get; set; } = new List<EntryEntity>();
    public ICollection<LabelEntity> Labels { get; set; } = new List<LabelEntity>();
    public ICollection<TemplateEntity> Templates { get; set; } = new List<TemplateEntity>();
}
