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

    [DoNotNotify]
    public Guid Id { get; set; }

    public LabelDetailModel? Label { get; set; }

    public LabelEditViewModel(ILabelClient labelClient)
    {
        _labelClient = labelClient;
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
}
