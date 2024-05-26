using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Helpers;
using Diary.Mappers;
using Diary.Models.Entry;
using Diary.Models.Label;
using Newtonsoft.Json;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

public partial class EntryListViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    private ICollection<LabelListModel> _labels;
    private EntryFilter _entryFilter { get; set; } = new();

    [ObservableProperty]
    private bool _filterSet;

    public EventCollection Events { get; set; } = [];
    public ObservableCollection<EntryListModel>? Items { get; set; }
    public ObservableCollection<EntryListModel>? SelectedDayEntries { get; set; } = [];
    public string? SelectedDate { get; set; } = null;

    public EntryListViewModel(IEntryClient entryClient, ILabelClient labelClient, IPopupService popupService)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _popupService = popupService;
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);

        _labels = await _labelClient.GetAllAsync();
        _entryFilter = LoadEntryFilter();

        FilterSet = IsFilterSet();
        Items = (await _entryClient.GetAllAsync(_entryFilter)).ToObservableCollection();

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
                var selectedDayEntries = (ICollection<EntryListModel>)dayEvents;
                SelectedDayEntries = selectedDayEntries.ToObservableCollection();
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

            Items = (await _entryClient.GetAllAsync(_entryFilter)).ToObservableCollection();
            Events = ConstructEventCollection(Items);
            SelectedDayEntries = Items;
        }
    }

    private void SaveEntryFilter(EntryFilter filter)
    {
        var filterJson = JsonConvert.SerializeObject(filter.MapToEntryFilterFromPreferences());

        Preferences.Set(Constants.EntryFilterPreferencesKey, filterJson);
    }

    private EntryFilter LoadEntryFilter()
    {
        var filterJson = Preferences.Get(Constants.EntryFilterPreferencesKey, string.Empty);

        EntryFilter defaultEntryFilter = new()
        {
            OrderByProperty = EntryFilterEnums.OrderByProperty.CreatedAt,
            OrderByDirection = EntryFilterEnums.OrderByDirection.Descending,
        };

        if (!string.IsNullOrEmpty(filterJson))
        {
            EntryFilterFromPreferences? entryFilterFromPreferences = JsonConvert.DeserializeObject<EntryFilterFromPreferences>(filterJson);

            return entryFilterFromPreferences != null
                ? entryFilterFromPreferences.MapToEntryFilter()
                : defaultEntryFilter;
        }

        return defaultEntryFilter;
    }

    private bool IsFilterSet()
    {
        return _entryFilter.DateFrom != null || _entryFilter.DateTo != null || _entryFilter.LabelsToShow?.Count > 0 || _entryFilter.MoodsToShow?.Count > 0;
    }
}
