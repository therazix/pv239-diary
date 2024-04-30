using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Models.Entry;
using Diary.Models.Label;
using Diary.ViewModels.Map;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(Entry), "entry")]
public partial class EntryEditViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    public EntryDetailModel Entry { get; set; } = null!;

    public ObservableCollection<LabelListModel> Labels { get; set; }

    public ObservableCollection<object> SelectedLabels { get; set; }

    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;
    public Color LocationTextColor { get; set; } = Color.FromArgb("#FF000000");

    public EntryEditViewModel(IEntryClient entryClient, ILabelClient labelClient, IPopupService popupService)
    {
        _entryClient = entryClient;
        _labelClient = labelClient;
        _popupService = popupService;
        SelectedLabels = new ObservableCollection<object>();
        Labels = new ObservableCollection<LabelListModel>();
    }

    public override async Task OnAppearingAsync()
    {
        var labels = await _labelClient.GetAllAsync();
        SelectedLabels = new ObservableCollection<object>(labels.Where(l => Entry.Labels.Select(el => el.Id).Contains(l.Id)));
        Labels = labels.ToObservableCollection();
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Entry.Id != Guid.Empty)
        {
            await _entryClient.DeleteAsync(Entry);
        }

        await Shell.Current.GoToAsync("//entries");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        Entry.Labels = new ObservableCollection<LabelListModel>(SelectedLabels.Select(l => (LabelListModel)l));

        await _entryClient.SetAsync(Entry);
        await Shell.Current.GoToAsync("//entries");
    }

    [RelayCommand]
    private Task ClearLocationAsync()
    {
        if (Entry != null)
        {
            Entry.Latitude = null;
            Entry.Longitude = null;
        }
        UpdateFormLocationInfo();
        return Task.CompletedTask;
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
        if (Entry != null && Entry.Latitude != null && Entry.Longitude != null)
        {
            pinLocation = new Location((double)Entry.Latitude, (double)Entry.Longitude);
        }

        var result = await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation));

        if (Entry != null && result is Location locationResult)
        {
            Entry.Latitude = locationResult.Latitude;
            Entry.Longitude = locationResult.Longitude;
        }
        UpdateFormLocationInfo();
    }

    private void UpdateFormLocationInfo()
    {
        IsLocationSet = Entry != null && (Entry.Latitude != null || Entry.Longitude != null);
        LocationText = IsLocationSet ? "Set" : "None";
        // TODO: Use a converter or predefined colors
        LocationTextColor = IsLocationSet ? Color.FromArgb("#FF1B9100") : Color.FromArgb("#FF000000");
    }
}
