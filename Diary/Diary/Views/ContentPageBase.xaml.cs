using Diary.Services.Interfaces;
using Diary.ViewModels.Interfaces;

namespace Diary.Views;

public abstract partial class ContentPageBase : ContentPage
{
    public static new readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(ContentPageBase), default(bool));

    public new bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    protected IViewModel viewModel { get; }
    private readonly IGlobalExceptionService _globalExceptionService;

    protected ContentPageBase(IViewModel viewModel, IGlobalExceptionService globalExceptionService)
    {
        _globalExceptionService = globalExceptionService;
        BindingContext = this.viewModel = viewModel;
        InitializeComponent();
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
