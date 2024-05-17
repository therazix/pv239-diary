using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.ViewModels.Interfaces;
using Newtonsoft.Json;
using Plugin.Maui.Calendar.Models;

namespace Diary.ViewModels.Entry;

[INotifyPropertyChanged]
public partial class EntryListViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    private ICollection<LabelListModel> _labels;
    private EntryFilter _entryFilter { get; set; } = new();

    [ObservableProperty]
    private EventCollection _events = [];

    [ObservableProperty]
    private ICollection<EntryListModel>? _items;

    [ObservableProperty]
    private ICollection<EntryListModel>? _selectedDayEntries = [];

    [ObservableProperty]
    private bool _filterSet;

    public string? SelectedDate { get; set; } = null;

    public EntryListViewModel(IEntryClient entryClient, ILabelClient labelClient, IPopupService popupService)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _popupService = popupService;
    }

    public async Task OnAppearingAsync()
    {
        _labels = await _labelClient.GetAllAsync();
        _entryFilter = LoadEntryFilter();
        FilterSet = IsFilterSet(); 

        Items = await _entryClient.GetAllAsync(_entryFilter);

        Events = ConstructEventCollection(Items);

        DaySelected(DateTime.Now.Date);
    }

    private EventCollection ConstructEventCollection(ICollection<EntryListModel> items)
    {
        var eventCollection = new EventCollection();

        foreach (var itemsByCreationDate in items.GroupBy(i => i.CreatedAt.Date))
        {
            eventCollection[itemsByCreationDate.Key] = itemsByCreationDate.ToList();
        }

        return eventCollection;
    }

    [RelayCommand]
    private void DaySelected(DateTime dateTime)
    {
        // When deselecting the date, show all entries
        if (SelectedDate == null)
        {
            SelectedDayEntries = Items;
        }
        else
        {
            var dayHasEvents = Events.TryGetValue(dateTime.Date, out var dayEvents);

            if (dayHasEvents)
            {
                SelectedDayEntries = (ICollection<EntryListModel>)dayEvents;
            }
            else
            {
                SelectedDayEntries = [];
            }
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//entries/detail", new Dictionary<string, object> { ["id"] = id });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await Shell.Current.GoToAsync("//entries/create");
    }

    [RelayCommand]
    private async Task DisplayFilterSortPopupAsync()
    {
        var result = await _popupService.ShowPopupAsync<FilterSortPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(_entryFilter, _labels));

        if (result is EntryFilter entryFilter)
        {
            _entryFilter = entryFilter;
            SaveEntryFilter(entryFilter);
            FilterSet = IsFilterSet();

            Items = await _entryClient.GetAllAsync(_entryFilter);
            Events = ConstructEventCollection(Items);
            SelectedDayEntries = Items;
        }
    }

    private void SaveEntryFilter(EntryFilter filter)
    {
        var filterJson = JsonConvert.SerializeObject(filter);
        Preferences.Set("EntryFilter", filterJson);
    }

    private EntryFilter LoadEntryFilter()
    {
        var filterJson = Preferences.Get("EntryFilter", string.Empty);

        EntryFilter defaultEntryFilter = new()
        {
            OrderByProperty = EntryFilterEnums.OrderByProperty.CreatedAt,
            OrderByDirection = EntryFilterEnums.OrderByDirection.Descending,
        };

        if (!string.IsNullOrEmpty(filterJson))
        {
            return JsonConvert.DeserializeObject<EntryFilter>(filterJson) ?? defaultEntryFilter;
        }

        return defaultEntryFilter;
    }

    private bool IsFilterSet()
    {
        return _entryFilter.DateFrom != null || _entryFilter.DateTo != null || _entryFilter.LabelsToShow?.Count > 0 || _entryFilter.MoodsToShow?.Count > 0;
    }
}
