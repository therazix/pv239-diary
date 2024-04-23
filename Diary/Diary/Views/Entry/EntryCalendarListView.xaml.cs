using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;

namespace Diary.Views.Entry;

public partial class EntryCalendarListView
{
    public EntryCalendarListView(EntryCalendarListViewModel viewModel, IGlobalExceptionService globalExceptionService) : base(viewModel, globalExceptionService)
    {
        InitializeComponent();
    }
}