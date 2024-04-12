using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class EntryEditView
{
    public EntryEditView(EntryEditViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}