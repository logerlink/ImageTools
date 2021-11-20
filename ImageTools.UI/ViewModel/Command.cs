using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ImageTools.UI.ViewModel
{
    /// <summary>
    /// 无参数  无返回值
    /// </summary>
    public class Command : ICommand
    {
        #region Fields
        private Action _execute;
        private Func<bool> _canExecute;
        #endregion 

        public Command(Action execute)
            : this(execute, null)
        {
        }
        public Command(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Member

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;

                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;

                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
        #endregion
    }
}
