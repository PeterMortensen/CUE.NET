using System;
using System.Windows.Input;

namespace AudioAnalyzer.RndStuff
{
    public class ActionCommand : ICommand
    {
        #region Properties & Fields

        private Action _action;
        private Func<bool> _canExecute;

        #endregion

        #region Constructors

        public ActionCommand(Action action, Func<bool> canExecute = null)
        {
            this._action = action;
            this._canExecute = canExecute;
        }

        #endregion

        #region Methods

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
