using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
//using System.Linq;
using System.Text;

namespace RenditionStudios.Library.NET.Observable
{
    public class ObservableList<T> : ObservableCollection<T>, IList<T>
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        //public event PropertyChangingEventHandler PropertyChanging;

        public new void Add(T item)
        {
            base.Add(item);

            // subscribe to PropertyChanged event

            if (item != null && item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += OnPropertyChanged;
        }

        public new void Clear()
        {
            base.Clear();

            // (un)subscribe to ItemChanged event
            foreach (var item in this)
            {
                if (item != null && item is INotifyPropertyChanged observableItem)
                    observableItem.PropertyChanged -= OnPropertyChanged;
            }
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);

            // (un)subscribe to ItemChanged event
            if (item != null && item is INotifyPropertyChanged observableItem)
                observableItem.PropertyChanged += OnPropertyChanged;
        }

        public new void Remove(T item)
        {
            // (un)subscribe to ItemChanged event
            if (item != null && item is INotifyPropertyChanged observableItem)
                observableItem.PropertyChanged -= OnPropertyChanged;

            base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            // (un)subscribe to ItemChanged event
            if (this[index] != null && this[index] is INotifyPropertyChanged observableItem)
                observableItem.PropertyChanged -= OnPropertyChanged;

            base.RemoveAt(index);
        }

        public void Replace(T oldItem, T newItem)
        {
            SetItem(IndexOf(oldItem), newItem);
        }

        public void ReplaceAt(int index, T item)
        {
            SetItem(index, item);
        }

        protected new void SetItem(int index, T item)
        {
            // variables
            T oldItem = this[index];
            bool oldItemIsObservable = oldItem != null && oldItem is INotifyPropertyChanged;
            T newItem = item;
            bool newItemIsObservable = newItem != null && newItem is INotifyPropertyChanged;

            // unsubscribe to ItemChanged event
            if (oldItemIsObservable)
                ((INotifyPropertyChanged)oldItem).PropertyChanged -= OnPropertyChanged;

            base.SetItem(index, item);

            // subscribe to ItemChanged event
            if (newItemIsObservable)
                ((INotifyPropertyChanged)newItem).PropertyChanged += OnPropertyChanged;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e); // sends "Count", "Item[]", etc
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e); // sends Item PropertyChangedEvent
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}
