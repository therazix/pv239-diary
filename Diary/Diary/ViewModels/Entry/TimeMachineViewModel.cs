using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using Diary.ViewModels.Interfaces;

namespace Diary.ViewModels.Entry;

[INotifyPropertyChanged]
public partial class TimeMachineViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;

    public string HeadingText { get; set; } = "Time machine is loading...";

    public ICollection<EntryListModel>? Entries { get; set; }

    public TimeMachineViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public async Task OnAppearingAsync()
    {
        Entries = await _entryClient.GetByDayFromPreviousYears(DateTime.Now);
        HeadingText = Entries.Count > 0 ? "This day in the past..." : "Time machine has nothing to show today...";
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//entries/timeMachine/detail", new Dictionary<string, object> { ["id"] = id });
    }
}
