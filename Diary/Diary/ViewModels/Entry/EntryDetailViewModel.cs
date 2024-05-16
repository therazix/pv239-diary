using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Entry;
using Diary.ViewModels.Map;
using Diary.ViewModels.Media;
using PropertyChanged;

namespace Diary.ViewModels.Entry;

[QueryProperty(nameof(Id), "id")]
public partial class EntryDetailViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;
    private readonly IPopupService _popupService;

    [DoNotNotify]
    public Guid Id { get; set; }

    public EntryDetailModel? Entry { get; set; }
    public bool IsLocationSet { get; set; } = false;
    public string LocationText { get; set; } = string.Empty;

    public EntryDetailViewModel(IEntryClient entryClient, IPopupService popupService)
    {
        _entryClient = entryClient;
        _popupService = popupService;
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);
        Entry = await _entryClient.GetByIdAsync(Id);
        UpdateFormLocationInfo();
    }

    [RelayCommand]
    private async Task GoToEditAsync()
    {
        if (Entry != null)
        {
            await Shell.Current.GoToAsync("/edit", new Dictionary<string, object> { ["entry"] = Entry });
        }
    }

    [RelayCommand]
    private async Task DisplayMediaPopupAsync(Guid id)
    {
        await _popupService.ShowPopupAsync<MediaPopupViewModel>(onPresenting: async viewModel => await viewModel.InitializeAsync(id));
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
        if (Entry != null && Entry.Latitude != null && Entry.Longitude != null)
        {
            pinLocation = new Location((double)Entry.Latitude, (double)Entry.Longitude);
        }

        await _popupService.ShowPopupAsync<MapPopupViewModel>(onPresenting: viewModel => viewModel.Initialize(pinLocation, userLocation, false));
    }

    private void UpdateFormLocationInfo()
    {
        IsLocationSet = Entry != null && (Entry.Latitude != null || Entry.Longitude != null);
        LocationText = IsLocationSet ? "" : "None";
    }
}
