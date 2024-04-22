using Diary.Services.Interfaces;
using Diary.ViewModels.ImportExport;

namespace Diary.Views.ImportExport;

public partial class ImportExportView
{
    public ImportExportView(ImportExportViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}