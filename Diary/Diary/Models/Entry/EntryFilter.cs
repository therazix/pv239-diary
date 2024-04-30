using static Diary.Enums.EntryFilterEnums;

namespace Diary.Models.Entry
{
    public class EntryFilter
    {
        public OrderByProperty? OrderByProperty;

        public OrderByDirection? OrderByDirection;
    }
}
