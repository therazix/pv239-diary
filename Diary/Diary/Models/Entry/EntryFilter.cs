using System.Collections.ObjectModel;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.Models.Entry
{
    public class EntryFilter
    {
        public OrderByProperty? OrderByProperty { get; set; }

        public OrderByDirection? OrderByDirection { get; set; }

        public ObservableCollection<object>? LabelsToShow { get; set; } = new();

        public ObservableCollection<object>? MoodsToShow { get; set; } = new();

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
