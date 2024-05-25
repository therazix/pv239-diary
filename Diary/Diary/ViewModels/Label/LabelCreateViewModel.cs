﻿using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Label;

namespace Diary.ViewModels.Label;

public partial class LabelCreateViewModel : ViewModelBase
{
    private readonly ILabelClient _labelClient;

    public LabelDetailModel? Label { get; set; }

    public LabelCreateViewModel(ILabelClient labelClient)
    {
        _labelClient = labelClient;
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
}
