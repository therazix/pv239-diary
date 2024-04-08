using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using PropertyChanged;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(Id), "id")]
public partial class EntryDetailViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;

    [DoNotNotify]
    public Guid Id { get; set; }

    public EntryDetailModel? Entry { get; set; }

    public EntryDetailViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
    {
        Entry = await _entryClient.GetByIdAsync(Id);
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Entry != null)
        {
            await Shell.Current.GoToAsync("/edit", new Dictionary<string, object> { ["entry"] = Entry });
        }
    }
}
