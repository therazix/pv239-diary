using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Entry;

namespace Diary.ViewModels.Entry;

public partial class TimeMachineViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    public string HeadingText { get; set; } = string.Empty;
    public ICollection<EntryListModel>? Entries { get; set; }

    public TimeMachineViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
    {
        var _ = new BusyIndicator(this);
        Entries = await _entryClient.GetByDayFromPreviousYearsAsync(DateTime.Now);
        HeadingText = Entries.Count > 0
            ? "You created the following entries on this day in previous years:"
            : "You haven't created any entries on this day in previous years.";
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//entries/timeMachine/detail", new Dictionary<string, object> { ["id"] = id });
    }
}
