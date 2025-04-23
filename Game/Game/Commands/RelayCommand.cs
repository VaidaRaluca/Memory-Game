using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> executeWithParam;
        private readonly Func<object, bool> canExecuteWithParam;

        private readonly Action executeWithoutParam;
        private readonly Func<bool> canExecuteWithoutParam;

        private readonly bool isParameterized;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecuteWithParam = canExecute;
            isParameterized = true;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            executeWithoutParam = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecuteWithoutParam = canExecute;
            isParameterized = false;
        }

        public bool CanExecute(object parameter)
        {
            return isParameterized
                ? (canExecuteWithParam?.Invoke(parameter) ?? true)
                : (canExecuteWithoutParam?.Invoke() ?? true);
        }

        public void Execute(object parameter)
        {
            if (isParameterized)
                executeWithParam(parameter);
            else
                executeWithoutParam();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }


}
