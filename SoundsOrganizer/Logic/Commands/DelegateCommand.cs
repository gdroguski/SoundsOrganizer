using System;
using System.Windows.Input;

namespace SoundsOrganizer.Logic.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;
        private bool _isEnabled;

        public DelegateCommand(Action action)
        {
            _action = action;
            _isEnabled = true;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public bool CanExecute(object parameter)
        {
            return _isEnabled;
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                OnCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
