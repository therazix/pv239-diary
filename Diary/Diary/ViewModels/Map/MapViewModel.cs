using CommunityToolkit.Maui.Core.Extensions;
using Diary.Clients.Interfaces;
using Diary.Models.Pin;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Map;

public partial class MapViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;

    public bool IsLocationEnabled { get; set; } = false;
    public Location? CurrentLocation { get; set; }

    public ObservableCollection<PinModel> Pins { get; set; } = new ObservableCollection<PinModel>();

    public MapViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
    {
        IsLocationEnabled = await Helpers.LocationHelper.HasLocationPermissionAsync();
        if (IsLocationEnabled)
        {
            CurrentLocation = await Helpers.LocationHelper.GetAnyLocationAsync();
        }
        Pins = (await _entryClient.GetAllLocationPinsAsync()).ToObservableCollection();
    }
}
