using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Models.Template;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Diary.ViewModels.Template;

public partial class TemplateListViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;

    public ObservableCollection<TemplateListModel>? Items { get; set; }

    public ICommand GoToDetailCommand { get; set; }

    public TemplateListViewModel(ITemplateClient templateClient, ICommandFactory commandFactory)
    {
        _templateClient = templateClient;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public override async Task OnAppearingAsync()
    {
        Items = (await _templateClient.GetAllAsync()).ToObservableCollection();
    }

    private async Task GoToDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//templates/detail", new Dictionary<string, object> { ["id"] = id });
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await Shell.Current.GoToAsync("//templates/create");
    }
}
