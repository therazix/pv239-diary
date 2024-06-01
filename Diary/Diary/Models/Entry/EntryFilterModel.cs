using Diary.Enums;
using Diary.Models.Label;
using Diary.Models.Mood;
using System.Collections.ObjectModel;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.Models.Entry
{
    public record EntryFilterModel : ModelBase
    {
        public OrderByProperty? OrderByProperty { get; set; } = EntryFilterEnums.OrderByProperty.CreatedAt;

        public OrderByDirection? OrderByDirection { get; set; } = EntryFilterEnums.OrderByDirection.Descending;

        public ObservableCollection<LabelListModel>? LabelsToShow { get; set; } = new();

        public ObservableCollection<MoodSelectionModel>? MoodsToShow { get; set; } = new();

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public bool FavoriteOnly { get; set; }
    }
}
