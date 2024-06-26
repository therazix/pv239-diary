﻿using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
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
    private EntryFilterModel _entryFilter { get; set; } = new();

    public bool FavoriteFilterSet { get; set; }
    public bool FilterSet { get; set; }
    public EventCollection Events { get; set; } = [];
    public ObservableCollection<EntryListModel>? Items { get; set; }
    public ObservableCollection<EntryListModel>? SelectedDayEntries { get; set; } = [];
    public string? SelectedDate { get; set; } = null;
    public DateTime SelectedDateTime { get; set; } = DateTime.MinValue;

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
        FavoriteFilterSet = IsFavoriteFilterSet();
        Items = (await _entryClient.GetAllAsync(_entryFilter)).ToObservableCollection();

        Events = ConstructEventCollection(Items);

        DaySelected();
    }

    private EventCollection ConstructEventCollection(ICollection<EntryListModel> items)
    {
        var eventCollection = new EventCollection();

        foreach (var itemsByCreationDate in items.GroupBy(i => i.DateTime.Date))
        {
            eventCollection[itemsByCreationDate.Key] = itemsByCreationDate.ToList();
        }

        return eventCollection;
    }

    [RelayCommand]
    private void DaySelected()
    {
        if (DateTime.TryParse(SelectedDate, out var dateTime))
        {
            SelectedDateTime = dateTime;
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
        else
        {
            // When deselecting the date, show all entries
            SelectedDayEntries = Items;
            SelectedDateTime = DateTime.MinValue;
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//entries/detail", new Dictionary<string, object> { ["id"] = id });
    }

    [RelayCommand]
    private async Task GoToCreateAsync(DateTime dateTime)
    {
        await Shell.Current.GoToAsync("//entries/create", new Dictionary<string, object> { ["dateTime"] = dateTime });
    }

    [RelayCommand]
    private async Task DisplayFilterSortPopupAsync()
    {
        var result = await _popupService.ShowPopupAsync<FilterSortPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(_entryFilter, _labels));

        if (result is EntryFilterModel entryFilter)
        {
            await ApplyFilter(entryFilter);
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteFilter()
    {
        _entryFilter.FavoriteOnly = !_entryFilter.FavoriteOnly;

        await ApplyFilter(_entryFilter);
    }

    private async Task ApplyFilter(EntryFilterModel entryFilter)
    {
        using var _ = new BusyIndicator(this);

        _entryFilter = entryFilter;
        SaveEntryFilter(entryFilter);
        FilterSet = IsFilterSet();
        FavoriteFilterSet = IsFavoriteFilterSet();

        Items = (await _entryClient.GetAllAsync(_entryFilter)).ToObservableCollection();
        Events = ConstructEventCollection(Items);
        SelectedDayEntries = Items;
    }

    private void SaveEntryFilter(EntryFilterModel filter)
    {
        var filterJson = JsonConvert.SerializeObject(filter);

        Preferences.Set(Constants.EntryFilterPreferencesKey, filterJson);
    }

    private EntryFilterModel LoadEntryFilter()
    {
        var filterJson = Preferences.Get(Constants.EntryFilterPreferencesKey, string.Empty);

        if (!string.IsNullOrEmpty(filterJson))
        {
            EntryFilterModel? entryFilter = JsonConvert.DeserializeObject<EntryFilterModel>(filterJson);

            return entryFilter ?? new EntryFilterModel();
        }

        return new EntryFilterModel();
    }

    private bool IsFilterSet()
    {
        return _entryFilter.DateFrom != null || _entryFilter.DateTo != null || _entryFilter.LabelsToShow?.Count > 0 || _entryFilter.MoodsToShow?.Count > 0;
    }

    private bool IsFavoriteFilterSet() => _entryFilter.FavoriteOnly;
}
