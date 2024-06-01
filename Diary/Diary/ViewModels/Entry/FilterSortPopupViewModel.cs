using CommunityToolkit.Maui.Core.Extensions;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Mood;
using System.Collections.ObjectModel;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.ViewModels.Entry;
public partial class FilterSortPopupViewModel : ViewModelBase
{
    public EntryFilterModel EntryFilter { get; set; } = new();

    public ObservableCollection<object> LabelsToShow { get; set; } = new();
    public ObservableCollection<object> MoodsToShow { get; set; } = new();

    public delegate void OnFilterApplyFilterAction(EntryFilterModel entryFilter);
    public event OnFilterApplyFilterAction OnFilterApplyFilter;

    public bool FilterByDateEnabled { get; set; }

    public ObservableCollection<LabelListModel> Labels { get; set; } = new();

    public ObservableCollection<MoodSelectionModel> Moods { get; set; } =
    [
        new() { Mood = 1 },
        new() { Mood = 2 },
        new() { Mood = 3 },
        new() { Mood = 4 },
        new() { Mood = 5 }
    ];

    public IEnumerable<OrderByProperty> OrderByProperties { get; set; } = Enum.GetValues(typeof(OrderByProperty)).Cast<OrderByProperty>();
    public IEnumerable<OrderByDirection> OrderByDirections { get; set; } = Enum.GetValues(typeof(OrderByDirection)).Cast<OrderByDirection>();

    public FilterSortPopupViewModel()
    {
    }

    public void Initialize(EntryFilterModel entryFilter, ICollection<LabelListModel> labels)
    {
        List<LabelListModel> selectedLabels = entryFilter.LabelsToShow?.ToList() ?? new();
        LabelsToShow = new ObservableCollection<object>(labels.Where(l => selectedLabels.Select(sl => sl.Id).Contains(l.Id)));

        if (entryFilter.MoodsToShow != null)
        {
            MoodsToShow = entryFilter.MoodsToShow.Cast<object>().ToObservableCollection();
        }

        EntryFilter = entryFilter;
        FilterByDateEnabled = EntryFilter.DateFrom != null || EntryFilter.DateTo != null;
        Labels = labels.ToObservableCollection();
    }

    public void SaveSelectedProperties()
    {
        EntryFilter.LabelsToShow = LabelsToShow.Any()
            ? new ObservableCollection<LabelListModel>(LabelsToShow.Select(l => (LabelListModel)l))
            : null;

        EntryFilter.MoodsToShow = MoodsToShow.Any()
            ? new ObservableCollection<MoodSelectionModel>(MoodsToShow.Select(m => (MoodSelectionModel)m))
            : null;
    }

    public EntryFilterModel GetEntryFilter()
    {
        if (FilterByDateEnabled)
        {
            EntryFilter.DateFrom = EntryFilter.DateFrom ?? DateTime.Now.Date;
            EntryFilter.DateTo = EntryFilter.DateTo ?? DateTime.Now.Date;
        }
        else
        {
            EntryFilter.DateFrom = null;
            EntryFilter.DateTo = null;
        }

        return EntryFilter;
    }
}
