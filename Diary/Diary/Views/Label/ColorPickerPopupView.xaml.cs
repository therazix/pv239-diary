using CommunityToolkit.Maui.Views;
using Diary.ViewModels.Label;

namespace Diary.Views.Label;

public partial class ColorPickerPopupView : Popup
{
    private ColorPickerPopupViewModel _viewModel { get; }

    public ColorPickerPopupView(ColorPickerPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        AdjustPopupSize();
    }

    private void AdjustPopupSize()
    {
        var window = Application.Current?.Windows[0];
        if (window != null)
        {
            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
            {
                var width = window.Width;
                var height = window.Height;
                this.Size = new Size(width, height);
                ColorPicker.WidthRequest = width * 0.9;
                ColorPicker.HeightRequest = height * 0.5;
            }
            else
            {
                var width = window.Width * 0.9;
                var height = window.Height * 0.9;
                this.Size = new Size(width, height);
                ColorPicker.WidthRequest = width * 0.5;
                ColorPicker.HeightRequest = height * 0.9;
            }
        }
    }

    private async void OnSaveButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(_viewModel.Color, cts.Token);
    }

    private async void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(null, cts.Token);
    }
}