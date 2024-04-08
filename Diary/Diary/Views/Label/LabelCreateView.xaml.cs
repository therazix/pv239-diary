using Diary.Services.Interfaces;
using Diary.ViewModels.Label;

namespace Diary.Views.Label;

public partial class LabelCreateView
{
    public LabelCreateView(LabelCreateViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}