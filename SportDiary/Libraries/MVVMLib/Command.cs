using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMLib
{
    public class Command : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }
        public Command(Action<object> execute)
          : this(execute, null)
        { }

        private  EventHandler canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add { canExecuteChanged += value; }
            remove { canExecuteChanged -= value; }
        }

        //#pragma warning restore IDE1006

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;
            execute(parameter);
        }
 
        //public void RaiseCanExecuteChanged()
        //{
        //    canExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}
    }
}
