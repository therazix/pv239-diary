using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Label;
using PropertyChanged;

namespace Diary.ViewModels.Label;

[QueryProperty(nameof(Id), "id")]
public partial class LabelEditViewModel : ViewModelBase
{
    private readonly ILabelClient _labelClient;
    private readonly IPopupService _popupService;

    [DoNotNotify]
    public Guid Id { get; set; }

    public LabelDetailModel? Label { get; set; }

    public LabelEditViewModel(ILabelClient labelClient, IPopupService popupService)
    {
        _labelClient = labelClient;
        _popupService = popupService;
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);
        Label = await _labelClient.GetByIdAsync(Id);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Label != null)
        {
            await _labelClient.DeleteAsync(Label);
        }
        await Shell.Current.GoToAsync("../");
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
