using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Models.Label;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace Diary.ViewModels.Label;

public partial class LabelListViewModel : ViewModelBase
{
    private readonly ILabelClient _labelClient;

    public ObservableCollection<LabelListModel>? Items { get; set; }

    public ICommand GoToDetailCommand { get; set; }

    public LabelListViewModel(ILabelClient labelClient, ICommandFactory commandFactory)
    {
        _labelClient = labelClient;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public override async Task OnAppearingAsync()
    {
        Items = (await _labelClient.GetAllAsync()).ToObservableCollection();
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
