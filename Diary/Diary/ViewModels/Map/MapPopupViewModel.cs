using Diary.Models.Pin;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Map;
public partial class MapPopupViewModel : ViewModelBase
{
    public bool IsLocationEnabled { get; set; }
    public bool IsControlEnabled { get; set; }

    public ObservableCollection<PinModel> Pins { get; set; } = new ObservableCollection<PinModel>();

    public delegate void OnInitializeAction(Location? pinLocation, Location? userLocation);
    public event OnInitializeAction OnInitialize;

    public MapPopupViewModel()
    {
        IsLocationEnabled = false;
    }

    public void Initialize(Location? pinLocation = null, Location? userLocation = null, bool isControlEnabled = true)
    {
        IsControlEnabled = isControlEnabled;
        IsLocationEnabled = userLocation != null;
        SetLocation(pinLocation);

        OnInitialize?.Invoke(pinLocation, userLocation);
    }

    public void SetLocation(Location? pinLocation)
    {
        Pins.Clear();

        if (pinLocation != null)
        {
            Pins.Add(new PinModel()
            {
                EntryId = Guid.Empty,
                Title = "Selected location",
                Location = pinLocation
            });
        }
    }

    public Location? GetPinLocation() => Pins.FirstOrDefault()?.Location;
}
