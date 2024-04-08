using Diary.Services.Interfaces;
using System.Diagnostics;

namespace Diary.Services;
public class GlobalExceptionService : IGlobalExceptionService
{
    public void HandleException(Exception exception, string? source = null)
    {
        Debug.WriteLine($"*** Exception thrown: '{exception.Message}' ***");

        MainThread.InvokeOnMainThreadAsync(() =>
        {
            Application.Current?.MainPage?.DisplayAlert("Something went wrong", exception.Message, "OK");
        });
    }
}
