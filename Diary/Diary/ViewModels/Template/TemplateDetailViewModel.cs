using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Template;
using Diary.ViewModels.Map;
using PropertyChanged;

namespace Diary.ViewModels.Template;

[QueryProperty(nameof(Id), "id")]
public partial class TemplateDetailViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly IPopupService _popupService;

    [DoNotNotify]
    public Guid Id { get; set; }

    [ObservableProperty]
    private bool _showMood;

    [ObservableProperty]
    private bool _showLocation;

    [ObservableProperty]
    private bool _showLabels;

    public TemplateDetailModel? Template { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;

    public TemplateDetailViewModel(ITemplateClient templateClient, IPopupService popupService)
    {
        _templateClient = templateClient;
        _popupService = popupService;
    }

    public override async Task OnAppearingAsync()
    {
        Template = await _templateClient.GetByIdAsync(Id);

        ShowMood = Template?.Mood != 0;
        ShowLocation = Template?.Latitude != null && Template?.Longitude != null;
        ShowLabels = Template?.Labels.Count > 0;

        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Template != null)
        {
            await Shell.Current.GoToAsync("/edit", new Dictionary<string, object> { ["template"] = Template });
        }
    }

    [RelayCommand]
    private async Task DisplayMapPopupAsync()
    {
        Location? userLocation = null;
        if (await Helpers.LocationHelper.HasLocationPermission())
        {
            userLocation = await Helpers.LocationHelper.GetAnyLocationAsync();
        }

        Location? pinLocation = null;
        if (Template != null && Template.Latitude != null && Template.Longitude != null)
        {
            pinLocation = new Location((double)Template.Latitude, (double)Template.Longitude);
        }

        var result = await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation, false));

        if (Template != null && result is Location locationResult)
        {
            Template.Latitude = locationResult.Latitude;
            Template.Longitude = locationResult.Longitude;
        }
        UpdateFormLocationInfo();
    }

    private void UpdateFormLocationInfo()
    {
        IsLocationSet = Template != null && (Template.Latitude != null || Template.Longitude != null);
        LocationText = IsLocationSet ? "" : "None";
    }
}
