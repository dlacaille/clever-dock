using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CleverDock.Patterns
{
    public class ActionCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;

        private readonly Action<T> _execute;

        private bool _couldExecute;

        public ActionCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? (x => true);
            _couldExecute = true;
        }

        public event EventHandler CanExecuteChanged;

        public static implicit operator ActionCommand<T>(Action<T> execute)
        {
            return new ActionCommand<T>(execute);
        }
        public bool CanExecute(T parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(T parameter)
        {
            _execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            try
            {
                var canExecute = CanExecute((T)parameter);

                if (_couldExecute ^ canExecute)
                {
                    _couldExecute = canExecute;
                    OnCanExecuteChanged();
                }

                return canExecute;
            }
            catch (InvalidCastException)
            {
                if (_couldExecute)
                {
                    _couldExecute = false;
                    OnCanExecuteChanged();
                }

                return false;
            }
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        private void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }

    public class ActionCommand : ICommand
    {
        private readonly Func<bool> _canExecute;

        private readonly Action _execute;

        private bool _couldExecute;

        public ActionCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute ?? (() => true);
            _couldExecute = true;
        }

        public event EventHandler CanExecuteChanged;

        public static implicit operator ActionCommand(Action execute)
        {
            return new ActionCommand(execute);
        }

        public bool CanExecute()
        {
            return _canExecute();
        }

        public void Execute()
        {
            _execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            try
            {
                var canExecute = CanExecute();

                if (_couldExecute ^ canExecute)
                {
                    _couldExecute = canExecute;
                    OnCanExecuteChanged();
                }

                return canExecute;
            }
            catch (InvalidCastException)
            {
                if (_couldExecute)
                {
                    _couldExecute = false;
                    OnCanExecuteChanged();
                }

                return false;
            }
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        private void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
