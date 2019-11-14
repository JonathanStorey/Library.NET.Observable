using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Data;

namespace RenditionStudios.Library.NET.Observable
{
    // Future plans:
    // Add INotifyPropertyChanging support: will require update of ObservableDictionary and View
    // Add INotifyDataErrorInfo support: will we add rules to the View?... View.Rules.Add(ValidationRule rule)///? IList?



    public class View : Model
    {
        public IModel Model { get; protected set; }

        public Dictionary<string, ViewItem> Dictionary { get; private set; } = new Dictionary<string, ViewItem>(); // theoretically dictionary should allow faster lookup

        public ViewProperties Properties { get; private set; }

        public View(IModel model)
        {
            // Context = SynchronizationContext.Current;
            Model = model;
            Properties = new ViewProperties(this, Dictionary);
        }

        public virtual T Get<T>(T value = default(T), [CallerMemberName]string name = null)
        {
            // retrieve item if in dictionary
            if (name != null && Dictionary.ContainsKey(name) && Dictionary[name].Value is T obj)
                return obj;

            // or add default item if not exists
            else if (name != null && Set<T>(value, name))
                return value;

            // default value already present
            else if (name != null)
                return value;

            else
                throw new Exception("View.Get<" + typeof(T).ToString() + ">(): PropertyName is not set.");
        }

        public virtual bool Set<T>(T value, [CallerMemberName]string name = null)
        {
            if (Dictionary.ContainsKey(name) && Equals(Dictionary[name].Value, value)) // equality check?
                return false;

            if (Dictionary.ContainsKey(name))
                Dictionary[name].SetValue(value);

            else
            {
                Dictionary.Add(name, new ViewItem(name, value, this));
                Model.NotifyPropertyChanged(name);
            }
                
            return true;
        }
    }
}
