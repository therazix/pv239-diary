using Diary.Services.Interfaces;
using Diary.ViewModels.Label;

namespace Diary.Views.Label;

public partial class LabelEditView
{
    public LabelEditView(LabelEditViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}