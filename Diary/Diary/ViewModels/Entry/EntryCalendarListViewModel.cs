using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models;
using Diary.Models.Entry;
using Diary.ViewModels.Interfaces;
using Plugin.Maui.Calendar.Models;
using System.Windows.Input;

namespace Diary.ViewModels.Entry;

[INotifyPropertyChanged]
public partial class EntryCalendarListViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;

    [ObservableProperty]
    private EventCollection _events = [];

    [ObservableProperty]
    private ICollection<EntryListModel>? _items;

    [ObservableProperty]
    private ICollection<EntryListModel>? _selectedDayEntries = [];

    public string ShownDate { get; set; } = DateTime.Now.Date.ToString("yyyy-MM-dd");


    public EntryCalendarListViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public async Task OnAppearingAsync()
    {
        Items = await _entryClient.GetAllAsync();

        Events = ConstructEventCollection(Items);
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
    private async Task DaySelected(DateTime dateTime)
    {
        // TODO: Use tryGetValue !!!
        SelectedDayEntries = (ICollection<EntryListModel>)Events[dateTime.Date];
    }
}
