namespace Diary.ViewModels.Interfaces;
public interface IViewModel
{
    bool IsBusy { get; set; }

    Task OnAppearingAsync();
}
