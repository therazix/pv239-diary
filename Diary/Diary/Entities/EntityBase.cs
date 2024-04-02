using System.ComponentModel.DataAnnotations;

namespace Diary.Entities
{
    public class EntityBase
    {
        [Key]
        public Guid Id { get; set; }
    }
}
