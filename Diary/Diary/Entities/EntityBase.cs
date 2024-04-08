using SQLite;

namespace Diary.Entities
{
    public class EntityBase
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.Empty;
    }
}
