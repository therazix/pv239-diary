using Diary.Services.Interfaces;
using Diary.ViewModels.Template;

namespace Diary.Views.Template;

public partial class TemplateListView
{
    public TemplateListView(TemplateListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}