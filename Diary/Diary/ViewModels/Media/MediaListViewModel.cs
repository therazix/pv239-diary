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

    public ObservableCollection<MediaGroupModel> MediaGroups { get; set; } = new();

    public MediaListViewModel(IMediaClient mediaClient)
    {
        _mediaClient = mediaClient;
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
}
