using Diary.Models.Label;
using Diary.Models.Mood;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.Models.Entry
{
    public class EntryFilterFromPreferences
    {
        public OrderByProperty? OrderByProperty { get; set; }

        public OrderByDirection? OrderByDirection { get; set; }

        public ICollection<LabelListModel>? LabelsToShow { get; set; } = [];

        public ICollection<MoodSelectionModel>? MoodsToShow { get; set; } = [];

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
