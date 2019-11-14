using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

// may need to use BeginInvoke
// https://hk.saowen.com/a/3fbcc7150fca376011fc5297f22ac75d6e2d4859c350371e38a6394ec84425d4

namespace RenditionStudios.Library.NET.Observable
{
    public interface IViewCommand : IModel, ICommand { }

    public abstract class ViewCommand<TParam, TResult> : ViewModel, IViewCommand where TParam: INotifyPropertyChanged
    {
        public virtual TParam Parameter { get => View.Get<TParam>(); set => View.Set<TParam>(value); }

        public Task<TResult> Result { get => View.Get<Task<TResult>>(); set => View.Set<Task<TResult>>(value); }

        // public View View;

        public event EventHandler CanExecuteChanged;

        //public event PropertyChangedEventHandler PropertyChanged;

        public ViewCommand() : base()
        {
            //View = new View(this);
        }

        public virtual bool CanExecute(object parameter)
        {
            SetParameter(parameter);

            // disables if Task is running
            if (Result != null && Result.IsCompleted == false)
                return false;

            // else it sends to CanExecute for further instructions...
            else
                return CanExecute();
        }

        public virtual async void Execute(object parameter)
        {
            SetParameter(parameter);

            Result = Execute();
            await Result;

            // forces refresh of Result and Parameter
            NotifyPropertyChanged("Result"); // forces the view to refresh
            if (Parameter != null && Parameter is IModel model)
                model.NotifyPropertyChanged(null);
        }

        public override void NotifyPropertyChanged(string propertyName)
        {
            base.NotifyPropertyChanged(propertyName);

            try
            {
                if (Context != null && Context != SynchronizationContext.Current)
                {
                    Context.Post(o => CanExecuteChanged?.Invoke(this, EventArgs.Empty), null);
                }

                else
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(this.GetType().Name + "OnPropertyChanged(" + propertyName + ") failed: " + exception.Message);
            }
        }

        public abstract bool CanExecute();

        public abstract Task<TResult> Execute();

        protected virtual bool SetParameter(object parameter)
        {
            if (parameter != null) // parameter is set inside of XAML
            {
                try
                {
                    Parameter = (TParam)parameter; // equality check happening inside of View.Set()
                    return true;
                }
                catch (Exception inner)
                {
                    throw new Exception("Command.Execute(): unable to convert parameter<" + parameter.GetType().ToString() + "> to " + typeof(TParam).ToString(), inner);
                    //return false;
                }
            }

            else if (Parameter != null) // Parameter is set outside of XAML
                return true;

            else
                return false;
        }
    }
}
