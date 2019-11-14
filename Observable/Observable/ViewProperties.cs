using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenditionStudios.Library.NET.Observable
{
    public class ViewProperties : IDictionary<string, object>
    {
        public ICollection<string> Keys => _dictionary.Keys.ToList();

        public ICollection<object> Values => _dictionary.Values.ToList();

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public object this[string key] { get => _viewDictionary[key].Value; set => _view.Set(value, key); }

        private IReadOnlyDictionary<string, object> _dictionary => _viewDictionary.ToDictionary(x => x.Key, y => y.Value.Value);

        private readonly View _view;

        private readonly IDictionary<string, ViewItem> _viewDictionary;

        public ViewProperties(View view, IDictionary<string, ViewItem> viewDictionary)
        {
            _view = view;
            _viewDictionary = viewDictionary;
        }

        public void Add(string key, object value)
        {
            _view.Set(value, key);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, object>)_dictionary).ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _viewDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (this.ContainsKey(item.Key) && this[item.Key].Equals(item.Value))
                return this.Remove(item.Key);
            else
                return false;

        }

        public bool TryGetValue(string key, out object value)
        {
            return ((IDictionary<string, object>)_dictionary).TryGetValue(key, out value);
        }

        public void Clear()
        {
            _viewDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)_dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IDictionary<string, object>)_dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, object>)_dictionary).GetEnumerator();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public override bool Equals(object obj)
        //{
        //    var entity = _view.Model as ITableEntity;


        //    //if (obj != null && obj is ITableEntity entity)
        //    //{

        //    //}
        //}
    }
}
