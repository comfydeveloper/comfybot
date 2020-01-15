namespace ComfyBot.Application.Shared
{
    using System;
    using System.Windows.Input;

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
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}