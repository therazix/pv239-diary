using CommunityToolkit.Maui.Views;
using Diary.ViewModels.Map;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Diary.Views.Map;

public partial class MapPopupView : Popup
{
    private MapPopupViewModel _viewModel { get; }

    public MapPopupView(MapPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.OnInitialize += ChangeLocation;
        AdjustPopupSize();
    }

    private void ChangeLocation(Location? pinLocation, Location? userLocation)
    {
        if (pinLocation != null)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(pinLocation, Distance.FromKilometers(1)));
        }
        else if (userLocation != null)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(userLocation, Distance.FromKilometers(1)));
        }
    }

    private void AdjustPopupSize()
    {
        var window = Application.Current?.Windows[0];
        if (window != null)
        {
            var widthMultiplier = DeviceInfo.Current.Idiom == DeviceIdiom.Phone ? 1.0 : 0.9;
            var heightMultiplier = DeviceInfo.Current.Idiom == DeviceIdiom.Phone ? 1.0 : 0.9;

            double width = window.Width * widthMultiplier;
            double height = window.Height * heightMultiplier;

            this.Size = new Size(width, height);
        }
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        if (_viewModel.IsControlEnabled)
        {
            _viewModel.SetLocation(e.Location);
        }
    }

    private async void OnSaveButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(_viewModel.GetPinLocation(), cts.Token);
    }

    private async void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(null, cts.Token);
    }
}