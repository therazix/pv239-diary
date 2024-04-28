using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Models.Entry;
using Diary.ViewModels.Interfaces;
using Plugin.Maui.Calendar.Models;

namespace Diary.ViewModels.Entry;

[INotifyPropertyChanged]
public partial class EntryListViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;

    [ObservableProperty]
    private EventCollection _events = [];

    [ObservableProperty]
    private ICollection<EntryListModel>? _items;

    [ObservableProperty]
    private ICollection<EntryListModel>? _selectedDayEntries = [];

    public string? SelectedDate { get; set; } = null;


    public EntryListViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public async Task OnAppearingAsync()
    {
        var entryFilter = new EntryFilter
        {
            OrderByProperty = EntryFilterEnums.OrderByProperty.CreatedAt,
            OrderByDirection = EntryFilterEnums.OrderByDirection.Desc
        };

        Items = await _entryClient.GetAllAsync(entryFilter);

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
}
