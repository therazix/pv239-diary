using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Label;
using Diary.Models.Template;
using Diary.ViewModels.Map;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Template;

[QueryProperty(nameof(Template), "template")]
public partial class TemplateEditViewModel : ViewModelBase
{
    private readonly ITemplateClient _templateClient;
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    public bool PresetMood { get; set; }
    public bool PresetLocation { get; set; }

    public TemplateDetailModel Template { get; set; } = null!;
    public ObservableCollection<LabelListModel> Labels { get; set; }
    public ObservableCollection<object> SelectedLabels { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;
    public Color LocationTextColor { get; set; } = Color.FromArgb("#FF000000");

    public TemplateEditViewModel(ITemplateClient templateClient, ILabelClient labelClient, IPopupService popupService)
    {
        _templateClient = templateClient;
        _labelClient = labelClient;
        _popupService = popupService;
        SelectedLabels = new ObservableCollection<object>();
        Labels = new ObservableCollection<LabelListModel>();
    }

    public override async Task OnAppearingAsync()
    {
        PresetMood = Template.Mood != 0;
        PresetLocation = Template.Latitude != null && Template.Longitude != null;

        var labels = await _labelClient.GetAllAsync();
        SelectedLabels = new ObservableCollection<object>(labels.Where(l => Template.Labels.Select(el => el.Id).Contains(l.Id)));
        Labels = labels.ToObservableCollection();
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Template.Id != Guid.Empty)
        {
            await _templateClient.DeleteAsync(Template);
        }

        await Shell.Current.GoToAsync("//templates");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Template.Mood = PresetMood ? Template.Mood : 0;

        if (!PresetLocation)
        {
            Template.Latitude = null;
            Template.Longitude = null;
        }

        Template.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));

        await _templateClient.SetAsync(Template);
        await Shell.Current.GoToAsync("//templates");
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
        if (await Helpers.LocationHelper.HasLocationPermission())
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
