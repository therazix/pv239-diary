using Diary.Services.Interfaces;
using Diary.ViewModels.Mood;

namespace Diary.Views.Mood;

public partial class MoodListView
{
    public MoodListView(MoodListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}