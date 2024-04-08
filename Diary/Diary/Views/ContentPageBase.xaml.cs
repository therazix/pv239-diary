using Diary.Services.Interfaces;
using Diary.ViewModels.Interfaces;

namespace Diary.Views;

public abstract partial class ContentPageBase : ContentPage
{
    private readonly IGlobalExceptionService _globalExceptionService;
    protected IViewModel viewModel { get; }

    protected ContentPageBase(IViewModel viewModel, IGlobalExceptionService globalExceptionService)
    {
        _globalExceptionService = globalExceptionService;
        BindingContext = this.viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await viewModel.OnAppearingAsync();
        }
        catch (Exception e)
        {
            _globalExceptionService.HandleException(e);
        }
    }
}
