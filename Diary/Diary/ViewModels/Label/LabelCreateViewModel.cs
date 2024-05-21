using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Label;

namespace Diary.ViewModels.Label;

public partial class LabelCreateViewModel : ViewModelBase
{
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    public LabelDetailModel? Label { get; set; }

    public LabelCreateViewModel(ILabelClient labelClient, IPopupService popupService)
    {
        _labelClient = labelClient;
        _popupService = popupService;
    }

    public override Task OnAppearingAsync()
    {
        Label = new LabelDetailModel()
        {
            Id = Guid.Empty
        };
        return base.OnAppearingAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Label != null)
        {
            using var _ = new BusyIndicator(this);
            await _labelClient.SetAsync(Label);
        }
        await Shell.Current.GoToAsync("../");
    }

    [RelayCommand]
    private async Task SelectColorAsync()
    {
        var result = await _popupService.ShowPopupAsync<ColorPickerPopupViewModel>();
        if (Label != null && result != null && result is Color color)
        {
            Label.Color = color;
        }
    }
}
