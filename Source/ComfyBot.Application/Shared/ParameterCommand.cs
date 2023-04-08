using System;
using System.Windows.Input;

namespace ComfyBot.Application.Shared;

public class ParameterCommand : ICommand
{
    private readonly Action<object> action;
    private readonly Predicate<object> canExecute;

    public ParameterCommand(Action<object> action, Predicate<object> canExecute = null)
    {
        this.action = action;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        if (canExecute == null)
        {
            return true;
        }
        return canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        action(parameter);
    }

    public event EventHandler CanExecuteChanged;
}