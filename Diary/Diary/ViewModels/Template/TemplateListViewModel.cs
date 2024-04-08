using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Models.Template;
using Diary.ViewModels.Interfaces;
using System.Windows.Input;

namespace Diary.ViewModels.Template;

[INotifyPropertyChanged]
public partial class TemplateListViewModel : IViewModel
{
    private readonly ITemplateClient _templateClient;

    [ObservableProperty]
    private ICollection<TemplateListModel>? items;

    public ICommand GoToDetailCommand { get; set; }

    public TemplateListViewModel(ITemplateClient templateClient, ICommandFactory commandFactory)
    {
        _templateClient = templateClient;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public async Task OnAppearingAsync()
    {
        Items = await _templateClient.GetAllAsync();
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
