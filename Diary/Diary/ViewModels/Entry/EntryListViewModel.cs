using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Models.Entry;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

public partial class EntryListViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;

    public EventCollection Events { get; set; } = [];
    public ObservableCollection<EntryListModel>? Items { get; set; }
    public ObservableCollection<EntryListModel>? SelectedDayEntries { get; set; } = [];
    public string? SelectedDate { get; set; } = null;

    public EntryListViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
    {
        var entryFilter = new EntryFilter
        {
            OrderByProperty = EntryFilterEnums.OrderByProperty.CreatedAt,
            OrderByDirection = EntryFilterEnums.OrderByDirection.Desc
        };

        Items = (await _entryClient.GetAllAsync(entryFilter)).ToObservableCollection();

        Events = ConstructEventCollection(Items);

        // TODO: implement faster loading
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
}
