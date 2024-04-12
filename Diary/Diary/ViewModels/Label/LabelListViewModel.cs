using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Models.Label;
using Diary.ViewModels.Interfaces;
using System.Windows.Input;


namespace Diary.ViewModels.Label;

[INotifyPropertyChanged]
public partial class LabelListViewModel : IViewModel
{
    private readonly ILabelClient _labelClient;

    [ObservableProperty]
    private ICollection<LabelListModel>? items;

    public ICommand GoToDetailCommand { get; set; }

    public LabelListViewModel(ILabelClient labelClient, ICommandFactory commandFactory)
    {
        _labelClient = labelClient;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public async Task OnAppearingAsync()
    {
        Items = await _labelClient.GetAllAsync();
    }

    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//labels/edit", new Dictionary<string, object> { ["id"] = id });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await Shell.Current.GoToAsync("//labels/create");
    }
}
