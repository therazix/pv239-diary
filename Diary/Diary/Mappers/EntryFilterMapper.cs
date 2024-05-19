using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Mood;
using Riok.Mapperly.Abstractions;
using System.Collections.ObjectModel;

namespace Diary.Mappers
{
    [Mapper]
    public static partial class EntryFilterMapper
    {
        public static partial EntryFilter MapToEntryFilter(this EntryFilterFromPreferences entryFilterFromPreferences);

        public static partial EntryFilterFromPreferences MapToEntryFilterFromPreferences(this EntryFilter entryFilters);


        public static ObservableCollection<object> MapToObservableCollection(this ICollection<LabelListModel> labels)
        {
            return new ObservableCollection<object>(labels);
        }

        public static ObservableCollection<object> MapToObservableCollection(this ICollection<MoodSelectionModel> moods)
        {
            return new ObservableCollection<object>(moods);
        }

        public static ICollection<LabelListModel> MapToLabelListModel(this ObservableCollection<object> labels)
        {
            return new List<LabelListModel>(labels.Cast<LabelListModel>());
        }

        public static ICollection<MoodSelectionModel> MapToMoodSelectionModel(this ObservableCollection<object> moods)
        {
            return new List<MoodSelectionModel>(moods.Cast<MoodSelectionModel>());
        }
    }
}
