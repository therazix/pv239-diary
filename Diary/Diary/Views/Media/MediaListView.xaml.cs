using Diary.Services.Interfaces;
using Diary.ViewModels.Media;

namespace Diary.Views.Media;

public partial class MediaListView
{
    public MediaListView(MediaListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}