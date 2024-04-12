using Diary.Services.Interfaces;
using Diary.ViewModels.Template;

namespace Diary.Views.Template;

public partial class TemplateEditView
{
    public TemplateEditView(TemplateEditViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}