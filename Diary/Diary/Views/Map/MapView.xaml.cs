using Diary.Models.Pin;
using Diary.Services.Interfaces;
using Diary.ViewModels.Map;
using Microsoft.Maui.Controls.Maps;

namespace Diary.Views.Map;

public partial class MapView
{
    public MapView(MapViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }

    public async void OnPinClickedAsync(object sender, PinClickedEventArgs e)
    {
        e.HideInfoWindow = true;
        var pinModel = (PinModel)((Pin)sender).BindingContext; // Get original pin model
        await Shell.Current.GoToAsync("//entries/detail", new Dictionary<string, object> { ["id"] = pinModel.EntryId });
    }
}