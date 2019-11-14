using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RenditionStudios.Library.NET.Observable
{
    public class ViewItem
    {
        public string Name { get; private set; }

        public object Value { get; private set; }

        public IModel Model => View?.Model;

        // public bool HasChanges { get; set; }

        public bool IsProcessed { get; set; }

        public View View { get; private set; }

        public bool TryPropertyChange;

        public bool SetValue(object value)
        {
            if (!Equals(value, Value))
            {
                if (Value != null && Value is INotifyPropertyChanged unsubscribeValue)
                    unsubscribeValue.PropertyChanged -= ValueChanged;

                Value = value;

                if (Value != null && Value is INotifyPropertyChanged subscribeValue)
                    subscribeValue.PropertyChanged += ValueChanged;

                ValueChanged(this, new PropertyChangedEventArgs("Value"));
                    

                return true;
            }

            else return false;    
        }

        public ViewItem(string name, object value, View view)
        {
            Name = name;
            Value = value;
            View = view;

            if (Value != null && Value is INotifyPropertyChanged observableValue)
                observableValue.PropertyChanged += ValueChanged;
          
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null && sender is ViewItem item && item.Model != null && item.IsProcessed == false)
            {
                // send for processing
                item.IsProcessed = true;
                Model.NotifyPropertyChanged(item.Name);
                item.IsProcessed = false;
            }

            else if (sender != null &&  sender is ViewModel model) // recursion
            {
                ValueChanged(this, e);
            }
        }

        public override string ToString()
        {
            return this.Name + ": " + this.Value?.ToString();
        }
    }
}
