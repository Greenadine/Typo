using System;
using System.Windows.Input;

namespace Typo.ViewModel
{
    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public Command(Action execute) : this(execute, null) { }

        public Command(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }
    }
}
