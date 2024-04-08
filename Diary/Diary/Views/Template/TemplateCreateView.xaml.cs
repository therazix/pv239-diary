using Diary.Services.Interfaces;
using Diary.ViewModels.Template;

namespace Diary.Views.Template;

public partial class TemplateCreateView
{
    public TemplateCreateView(TemplateCreateViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}