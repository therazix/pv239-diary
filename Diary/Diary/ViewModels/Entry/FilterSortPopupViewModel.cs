using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.Models.Mood;
using static Diary.Enums.EntryFilterEnums;

namespace Diary.ViewModels.Entry;
public partial class FilterSortPopupViewModel : ViewModelBase
{
    [ObservableProperty]
    private EntryFilter _entryFilter = new();

    public EntryFilter EntryFilterOriginalState { get; set; } = new();

    public delegate void OnFilterApplyFilterAction(EntryFilter entryFilter);
    public event OnFilterApplyFilterAction OnFilterApplyFilter;

    public bool FilterByDate { get; set; }

    public ICollection<LabelListModel> Labels { get; set; }

    public ICollection<MoodSelectionModel> Moods { get; set; } =
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

    public void Initialize(EntryFilter entryFilter, ICollection<LabelListModel> labels)
    {
        EntryFilter = entryFilter;
        EntryFilterOriginalState = entryFilter;
        FilterByDate = EntryFilter.DateFrom != null && EntryFilter.DateTo != null;
        Labels = labels;
    }

    public EntryFilter GetEntryFilter()
    {
        if (!FilterByDate)
        {
            EntryFilter.DateFrom = null;
            EntryFilter.DateTo = null;
        }
        return EntryFilter;
    }

    [RelayCommand]
    private void ToggleDateFiltering()
    {
        if (FilterByDate)
        {
            EntryFilter.DateFrom = DateTime.Now.Date;
            EntryFilter.DateTo = DateTime.Now.Date;
        }
        else
        {
            EntryFilter.DateFrom = null;
            EntryFilter.DateTo = null;
        }
    }
}
