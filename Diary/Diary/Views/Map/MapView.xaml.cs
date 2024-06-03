using Diary.Models.Pin;
using Diary.Services.Interfaces;
using Diary.ViewModels.Map;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Diary.Views.Map;

public partial class MapView
{
    public MapView(MapViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();

        if (viewModel.CurrentLocation != null)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(viewModel.CurrentLocation, Distance.FromKilometers(1)));
        }
    }

    private async void OnPinClicked(object sender, PinClickedEventArgs e)
    {
        e.HideInfoWindow = true;
        var pinModel = (PinModel)((Pin)sender).BindingContext; // Get original pin model
        await Shell.Current.GoToAsync("//map/detail", new Dictionary<string, object> { ["id"] = pinModel.EntryId });
    }
}