using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;

namespace Diary.ViewModels.Entry;

public partial class TimeMachineViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    public string HeadingText { get; set; } = "Time machine is loading...";
    public ICollection<EntryListModel>? Entries { get; set; }

    public TimeMachineViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
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
