using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class EntryCreateView
{
    public EntryCreateView(EntryCreateViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}