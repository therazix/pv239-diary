using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Commands.Interfaces;
using Diary.Helpers;
using Diary.Models.Template;
using Diary.ViewModels.Map;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Diary.ViewModels.Template;

public partial class TemplateListViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly IPopupService _popupService;

    public ObservableCollection<TemplateListModel>? Items { get; set; }

    public ICommand GoToDetailCommand { get; set; }

    public TemplateListViewModel(ITemplateClient templateClient, ICommandFactory commandFactory, IPopupService popupService)
    {
        _templateClient = templateClient;
        _popupService = popupService;
        GoToDetailCommand = commandFactory.Create<Guid>(GoToDetailAsync);
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);
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

    [RelayCommand]
    private async Task DisplayMapPopupAsync(Guid templateId)
    {
        Location? userLocation = null;
        if (await LocationHelper.HasLocationPermissionAsync())
        {
            userLocation = await LocationHelper.GetAnyLocationAsync();
        }

        Location? pinLocation = null;
        TemplateListModel? template = Items?.FirstOrDefault(t => t.Id == templateId);

        if (template != null && template.Latitude != null && template.Longitude != null)
        {
            pinLocation = new Location((double)template.Latitude, (double)template.Longitude);
        }

        await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation, false));
    }
}
