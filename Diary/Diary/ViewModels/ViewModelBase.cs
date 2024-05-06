using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diary.ViewModels.Interfaces;

namespace Diary.ViewModels;

public abstract partial class ViewModelBase : ObservableObject, IViewModel
{
    public bool IsBusy { get; set; }

    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task GoBackAsync()
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            Shell.Current.SendBackButtonPressed();
        }

        return Task.CompletedTask;
    }
}
