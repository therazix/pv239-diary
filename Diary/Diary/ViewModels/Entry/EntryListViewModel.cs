using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Models.Entry;
using Diary.ViewModels.Interfaces;
using System.Windows.Input;

namespace Diary.ViewModels.Entry;

[INotifyPropertyChanged]
public partial class EntryListViewModel : IViewModel
{
    private readonly IEntryClient _entryClient;

    [ObservableProperty]
    private ICollection<EntryListModel>? items;

    public ICommand GoToDetailCommand { get; set; }

    public EntryListViewModel(IEntryClient entryClient, ICommandFactory commandFactory)
    {
        _entryClient = entryClient;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public async Task OnAppearingAsync()
    {
        Items = await _entryClient.GetAllAsync();
    }

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
