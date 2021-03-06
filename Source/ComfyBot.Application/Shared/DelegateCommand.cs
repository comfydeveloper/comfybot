﻿namespace ComfyBot.Application.Shared
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private readonly Action action;

        private readonly Predicate<object> canExecute;

#pragma warning disable 67
        public event EventHandler CanExecuteChanged
        {
            [ExcludeFromCodeCoverage]
            add { CommandManager.RequerySuggested += value; }
            [ExcludeFromCodeCoverage]
            remove { CommandManager.RequerySuggested -= value; }
        }
#pragma warning restore 67

        public DelegateCommand(Action action, Predicate<object> canExecute = null)
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

        public void Execute(object parameter = null)
        {
            this.action();
        }
    }
}