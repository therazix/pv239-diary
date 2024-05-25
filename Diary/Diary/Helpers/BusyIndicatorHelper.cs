using Diary.ViewModels;
using Diary.ViewModels.Interfaces;

namespace Diary.Helpers;
public class BusyIndicator : IDisposable
{
    private readonly IViewModel _viewModel;
    private readonly string _isBusyPropertyName;

    public BusyIndicator(IViewModel viewModel, string isBusyPropertyName = nameof(ViewModelBase.IsBusy))
    {
        _viewModel = viewModel;
        _isBusyPropertyName = isBusyPropertyName;
        SetIsBusy(true);
    }

    public void Dispose()
    {
        SetIsBusy(false);
    }

    private void SetIsBusy(bool value)
    {
        var property = _viewModel.GetType().GetProperty(_isBusyPropertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(_viewModel, value);
        }
    }

}
