using System.Runtime.CompilerServices;

namespace Diary.Services.Interfaces;
public interface IGlobalExceptionService
{
    void HandleException(Exception exception, [CallerMemberName] string? source = null);
}
