using Diary.Services.Interfaces;
using Java.Lang;

namespace Diary.Platforms;
public class GlobalUncaughtExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
{
    private readonly IGlobalExceptionService globalExceptionService;

    public GlobalUncaughtExceptionHandler(IGlobalExceptionService globalExceptionService)
    {
        this.globalExceptionService = globalExceptionService;
    }

    public void UncaughtException(Java.Lang.Thread t, Throwable e)
    {
        globalExceptionService.HandleException(e, nameof(GlobalUncaughtExceptionHandler));
    }
}
