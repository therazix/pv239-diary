using Diary.Clients.Interfaces;
using Diary.Models.Pin;

namespace Diary.ViewModels.Map;

public partial class MapViewModel : ViewModelBase
{
    private readonly IEntryClient _entryClient;

    public bool IsLocationEnabled { get; set; } = false;

    public IEnumerable<PinModel> Pins { get; set; } = new List<PinModel>();

    public MapViewModel(IEntryClient entryClient)
    {
        _entryClient = entryClient;
    }

    public override async Task OnAppearingAsync()
    {
        IsLocationEnabled = await HasLocationPermission();
        Pins = await _entryClient.GetAllLocationPinsAsync();
    }

    private async Task<bool> HasLocationPermission()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }
}
