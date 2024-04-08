using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class EntryDetailView
{
    public EntryDetailView(EntryDetailViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}