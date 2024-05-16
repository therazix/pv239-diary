using CommunityToolkit.Maui.Views;
using Diary.ViewModels.Media;

namespace Diary.Views.Media;

public partial class MediaPopupView : Popup
{
    public MediaPopupView(MediaPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        AdjustPopupSize();
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

    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(null, cts.Token);
    }
}