using CommunityToolkit.Maui.Views;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class FilterSortPopupView : Popup
{
    private FilterSortPopupViewModel _viewModel { get; }

    public FilterSortPopupView(FilterSortPopupViewModel viewModel)
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
            double width = window.Width * 0.9;
            double height = window.Height * 0.9;

            this.Size = new Size(width, height);
        }
    }

    private async void OnApplyFilterButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(_viewModel.EntryFilter, cts.Token);
    }

    private async void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(Constants.CancellationTokenDelay);
        await CloseAsync(_viewModel.EntryFilterOriginalState, cts.Token);
    }
}