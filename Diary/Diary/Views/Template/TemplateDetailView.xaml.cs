using Diary.Services.Interfaces;
using Diary.ViewModels.Template;

namespace Diary.Views.Template;

public partial class TemplateDetailView
{
    public TemplateDetailView(TemplateDetailViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}