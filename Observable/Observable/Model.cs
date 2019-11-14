using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenditionStudios.Library.NET.Observable
{
    public interface IModel : INotifyPropertyChanged // RENAME IMODEL?
    {
        void NotifyPropertyChanged(string propertyName);
    }

    public abstract class Model : IModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual SynchronizationContext Context { get; set; } = SynchronizationContext.Current;

        public virtual void NotifyPropertyChanged(string propertyName) // any advantage to using a Task
        {
            var args = new PropertyChangedEventArgs(propertyName);

            try
            {
                if (Context != null && Context != SynchronizationContext.Current)
                    Context.Post(o => PropertyChanged?.Invoke(this, args), null);

                else
                    PropertyChanged?.Invoke(this, args);
                //return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                string errorMessage = this.GetType().Name + "OnPropertyChanged(" + args?.PropertyName + ") failed. ";
                Debug.WriteLine(errorMessage + exception.Message);
                //return Task.FromException(exception);
            }
        }
    }
}
