using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Label;
using Diary.Models.Template;
using Diary.ViewModels.Map;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Template;

public partial class TemplateCreateViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    public bool PresetMood { get; set; } = true;
    public bool PresetLocation { get; set; } = true;
    public TemplateDetailModel? Template { get; set; }
    public ObservableCollection<LabelListModel> Labels { get; set; }
    public ObservableCollection<object> SelectedLabels { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;
    public Color LocationTextColor { get; set; } = Color.FromArgb("#FF000000");

    public TemplateCreateViewModel(ITemplateClient templateClient, ILabelClient labelClient, IPopupService popupService)
    {
        _templateClient = templateClient;
        _labelClient = labelClient;
        _popupService = popupService;
        Labels = new ObservableCollection<LabelListModel>();
        SelectedLabels = new ObservableCollection<object>();
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);

        Template = new TemplateDetailModel()
        {
            Id = Guid.Empty
        };

        var labels = await _labelClient.GetAllAsync();
        Labels = labels.ToObservableCollection();
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Template != null)
        {
            Template.Mood = PresetMood ? Template.Mood : 0;

            if (!PresetLocation)
            {
                Template.Latitude = null;
                Template.Longitude = null;
            }

            using var _ = new BusyIndicator(this);
            Template.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));
            await _templateClient.SetAsync(Template);
        }
        await Shell.Current.GoToAsync("../");
    }

    [RelayCommand]
    private void ClearLocation()
    {
        if (Template != null)
        {
            Template.Latitude = null;
            Template.Longitude = null;
        }
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task DisplayMapPopupAsync()
    {
        Location? userLocation = null;
        if (await Helpers.LocationHelper.HasLocationPermissionAsync())
        {
            userLocation = await Helpers.LocationHelper.GetAnyLocationAsync();
        }

        Location? pinLocation = null;
        if (Template != null && Template.Latitude != null && Template.Longitude != null)
        {
            pinLocation = new Location((double)Template.Latitude, (double)Template.Longitude);
        }

        var result = await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation));

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
        LocationText = IsLocationSet ? "Set" : "None";
        // TODO: Use a converter or predefined colors
        LocationTextColor = IsLocationSet ? Color.FromArgb("#FF1B9100") : Color.FromArgb("#FF000000");
    }
}