using Diary.Entities;
using System.ComponentModel.DataAnnotations;

namespace Diary.Enums
{
    public static class EntryFilterEnums
    {
        public enum OrderByProperty
        {
            [Display(Name = nameof(EntryEntity.Title))]
            Title,

            [Display(Name = nameof(EntryEntity.CreatedAt))]
            CreatedAt,

            [Display(Name = nameof(EntryEntity.Mood))]
            Mood,
        }

        public enum OrderByDirection
        {
            Asc,
            Desc,
        }

        public static string GetEnumDisplayName(Enum value)
        {
            return Enum.GetName(value.GetType(), value) ?? "";
        }
    }
}
