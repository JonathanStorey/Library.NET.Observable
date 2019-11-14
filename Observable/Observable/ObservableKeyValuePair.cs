using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace RenditionStudios.Library.NET.Observable
{
    public interface IObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
    {
        TKey Key { get; }

        TValue Value { get; set; }

        KeyValuePair<TKey, TValue> KeyValuePair { get; }

        void SetValue(TKey key, TValue value);
    }

    public class ObservableKeyValuePair<TKey, TValue> : IObservableKeyValuePair<TKey, TValue>
    {
        public TKey Key => KeyValuePair.Key;

        public TValue Value { get => KeyValuePair.Value; set => SetValue(value); }

        public KeyValuePair<TKey, TValue> KeyValuePair { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableKeyValuePair() { }

        public ObservableKeyValuePair(TKey key, TValue value)
        {
            SetValue(key, value);
        }

        public ObservableKeyValuePair(KeyValuePair<TKey, TValue> item)
        {
            SetValue(item.Key, item.Value);
        }

        public void SetValue(TKey key, TValue value)
        {
            if (KeyValuePair.Value != null && KeyValuePair.Value is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Value).PropertyChanged -= ValuePropertyChanged;

            KeyValuePair = new KeyValuePair<TKey, TValue>(key, value);
            ValuePropertyChanged(this.KeyValuePair, new PropertyChangedEventArgs("Value"));

            if (KeyValuePair.Value != null && KeyValuePair.Value is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Value).PropertyChanged += ValuePropertyChanged;
        }

        private void SetValue(TValue value)
        {
            if (KeyValuePair.Value != null && KeyValuePair.Value is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Value).PropertyChanged -= ValuePropertyChanged;

            KeyValuePair = new KeyValuePair<TKey, TValue>(Key, value);
            ValuePropertyChanged(this.KeyValuePair, new PropertyChangedEventArgs("Value"));

            if (KeyValuePair.Value != null && KeyValuePair.Value is INotifyPropertyChanged)
                ((INotifyPropertyChanged)Value).PropertyChanged += ValuePropertyChanged;
        }

        private void ValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ObservableKeyValuePair.OnPropertyChanged(" + e.PropertyName + ") => PropertyChanged.Invoke FAILED.");
                throw ex;
            }
        }
    }

    
}
