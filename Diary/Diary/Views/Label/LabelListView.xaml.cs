using Diary.Services.Interfaces;
using Diary.ViewModels.Label;

namespace Diary.Views.Label;

public partial class LabelListView
{
    public LabelListView(LabelListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}
