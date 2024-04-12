using System.Windows.Input;

namespace Diary.Commands.Interfaces;
public interface ICommandFactory
{
    ICommand Create(Action action);
    Command<T> Create<T>(Func<T, Task> action, Func<T, bool>? canExecute = null);
}
