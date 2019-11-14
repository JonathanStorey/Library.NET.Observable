using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RenditionStudios.Library.NET.Observable
{
    public abstract class Command<TParameter, TResult> : Model, ICommand where TResult : Task
    {
        public TParameter Parameter { get => _view.Get<TParameter>(); set => _view.Set(value); }

        public TResult Result { get => _view.Get<TResult>(); set => _view.Set(value); }

        private View _view;

        public Command() : base()
        {
            _view = new View(this);
        }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            Parameter = (TParameter)parameter;

            if (Result != null && Result.IsCompleted == false) // CanExecute = false if Task is runnnig
                return false;

            return CanExecute(); // runs and returns CanExecute
        }

        public abstract bool CanExecute();

        public virtual async void Execute(object parameter)
        {
            Parameter = (TParameter)parameter;
            Result = Execute(); // should fire view update
            await Result;
            NotifyPropertyChanged("Result"); // fire view update
            if (Parameter != null && Parameter is IModel model) model.NotifyPropertyChanged(null);
        }

        public abstract TResult Execute();

        public override void NotifyPropertyChanged(string propertyName)
        {
            base.NotifyPropertyChanged(propertyName);

            try
            {
                if (Context != null && Context != SynchronizationContext.Current)
                    Context.Post(o => CanExecuteChanged?.Invoke(this, new EventArgs()), null);

                else
                    CanExecuteChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception exception)
            {
                string errorMessage = this.GetType().Name + "OnPropertyChanged(" + propertyName + ") failed: " + exception.Message;
                Debug.WriteLine(errorMessage + exception.Message);
            }
        }
    }

    public abstract class Command<TParameter> : Command<TParameter, Task> { }

    public abstract class Command : Command<object, Task> { }
}
