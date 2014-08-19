using System;
using System.Windows.Input;

namespace SharpXMPP.WPF.Helpers
{
    public class DelegateCommand<T> : ICommand
    {
        readonly Action<T> _action;
        readonly Func<bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<bool> canExecute)
        {
            _action = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
