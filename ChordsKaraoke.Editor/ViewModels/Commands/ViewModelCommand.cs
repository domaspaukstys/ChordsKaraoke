using System;
using System.Windows.Input;

namespace ChordsKaraoke.Editor.ViewModels.Commands
{
    public abstract class ViewModelCommand<T> : ICommand
        where T: ViewModel
    {
        protected ViewModelCommand(T model)
        {
            ViewModel = model;
        }
        public T ViewModel { get; private set; }

        public virtual bool CanExecute()
        {
            return CanExecute(null);
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute()
        {
            Execute(null);
        }

        public abstract void Execute(object parameter);


        public event EventHandler CanExecuteChanged;

        public virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
