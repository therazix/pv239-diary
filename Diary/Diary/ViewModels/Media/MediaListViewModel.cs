using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Models.Media;
using System.Collections.ObjectModel;

namespace Diary.ViewModels.Media;

public partial class MediaListViewModel : ViewModelBase
{
    private readonly IMediaClient _mediaClient;
    private readonly IPopupService _popupService;

    public ObservableCollection<MediaGroupModel> MediaGroups { get; set; } = new();

    public MediaListViewModel(IMediaClient mediaClient, IPopupService popupService)
    {
        _mediaClient = mediaClient;
        _popupService = popupService;
    }

    public override async Task OnAppearingAsync()
    {
        using var _ = new BusyIndicator(this);
        var media = await _mediaClient.GetAllAsync();
        MediaGroups = media.GroupBy(
            m => m.CreatedAt.Date,
            m => m,
            (date, media) => new MediaGroupModel
            {
                Date = date,
                Media = media.ToObservableCollection()
            }).OrderByDescending(m => m.Date)
            .ToObservableCollection();
    }

    [RelayCommand]
    private async Task GoToEntryDetailAsync(Guid id)
    {
        await Shell.Current.GoToAsync("//entries/detail", new Dictionary<string, object> { ["id"] = id });
    }

    [RelayCommand]
    private async Task DisplayMediaPopupAsync(Guid id)
    {
        await _popupService.ShowPopupAsync<MediaPopupViewModel>(onPresenting: async viewModel => await viewModel.InitializeAsync(id));
    }
}
