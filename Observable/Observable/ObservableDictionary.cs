using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RenditionStudios.Library.NET.Observable
{
    // https://www.codeproject.com/Articles/1004644/ObservableCollection-Simply-Explained

    public class ObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue, ObservableKeyValuePair<TKey, TValue>> { }

    public class ObservableDictionary<TKey, TValue, TPair> : IDictionary<TKey, TValue>, INotifyPropertyChanged, INotifyCollectionChanged where TPair : IObservableKeyValuePair<TKey, TValue>, new()
    {
        public ObservableDictionary()
        {
            _list.CollectionChanged += OnCollectionChanged;
            _list.PropertyChanged += OnPropertyChanged;
        }

        protected ObservableList<TPair> _list = new ObservableList<TPair>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public TValue this[TKey key] { get => _list.Single(x => x.Key.Equals(key)).Value; set => SetItem(key, value); }

        public ICollection<TKey> Keys { get; set; } = new ObservableList<TKey>();

        public ICollection<TValue> Values { get; set; } = new ObservableList<TValue>();

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        protected virtual void SetItem(TKey key, TValue value)
        {
            if (this.ContainsKey(key))
            {
                var item = _list.Single(x => x.Key.Equals(key));
                item.Value = value; // should trigger PropertyChanged event inside ObservableKeyValuePair
            }
            else
                this.Add(key, value);
        }

        public void Add(TKey key, TValue value)
        {
            if (key != null && !this.ContainsKey(key))
            {
                var pair = new TPair();
                pair.SetValue(key, value);

                _list.Add(pair);
                Keys.Add(key);
                OnPropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                Values.Add(value);
                OnPropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }

            else if (key == null)
            {
                throw new Exception("ObservableDictionary: Key must not be null.");
            }

            else if (this.ContainsKey(key))
            {
                throw new Exception("ObservableDictionary: Duplicate key: " + key.ToString());
            }

            else
            {
                throw new Exception("ObservableDictionary: Add method failed.");
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var pair = new TPair();
            pair.SetValue(item.Key, item.Value);
            _list.Add(pair);
        }

        public void Clear()
        {
            _list.Clear();
            Keys.Clear();
            OnPropertyChanged(this, new PropertyChangedEventArgs("Keys"));
            Values.Clear();
            OnPropertyChanged(this, new PropertyChangedEventArgs("Values"));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _list.Select(x => x.KeyValuePair).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new Exception("Key can not be null");

            return _list.Any(x => x.Key != null && x.Key.Equals(key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _list.Select(x => x.KeyValuePair).ToList().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _list.Select(x => x.KeyValuePair).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            var item = _list.SingleOrDefault(x => x.Key.Equals(key));

            if (item != null)
            {
                var index = _list.IndexOf(item);
                _list.RemoveAt(index);
                ((ObservableList<TKey>)(Keys)).RemoveAt(index);
                OnPropertyChanged(this, new PropertyChangedEventArgs("Keys"));
                ((ObservableList<TValue>)(Keys)).RemoveAt(index);
                OnPropertyChanged(this, new PropertyChangedEventArgs("Values"));
                return true;
            }
            else
                return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var pair = _list.SingleOrDefault(x => x.KeyValuePair.Equals(item));

            if (pair != null)
            {
                this.Remove(pair.Key);
                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = _list.Single(x => x.Key != null && x.Key.Equals(key)).Value;
                return true;
            }
            catch (Exception)
            {
                value = default(TValue);
                return false;
            }
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (var item in this)
            {
                dictionary.Add(item.Key, item.Value);
            }

            return dictionary;
        }

        public static implicit operator Dictionary<TKey, TValue>(ObservableDictionary<TKey, TValue, TPair> obsDictionary)
        {
            return obsDictionary.ToDictionary();
        }
    }
}
