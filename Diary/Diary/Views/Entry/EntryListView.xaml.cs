using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class EntryListView
{
    public EntryListView(EntryListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}