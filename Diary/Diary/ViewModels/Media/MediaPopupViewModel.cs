using Diary.Clients.Interfaces;
using Diary.Enums;
using Diary.Models.Media;

namespace Diary.ViewModels.Media;

public partial class MediaPopupViewModel : ViewModelBase
{
    private readonly IMediaClient _mediaClient;

    public MediaModel? Media { get; set; }
    public bool IsImage { get; set; }
    public bool IsVideo { get; set; }

    public MediaPopupViewModel(IMediaClient mediaClient)
    {
        _mediaClient = mediaClient;
    }

    public async Task InitializeAsync(Guid id)
    {
        Media = await _mediaClient.GetByIdAsync(id);
        IsImage = Media?.MediaType == MediaType.Image;
        IsVideo = Media?.MediaType == MediaType.Video;
    }
}
