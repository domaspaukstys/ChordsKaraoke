using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ChordsKaraoke.Editor.Models
{
    public class ObservableSortedList<T> : IList<T>, INotifyCollectionChanged
        where T : INotifyPropertyChanged
    {
        private readonly List<T> _items;
        private readonly IComparer<T> _comparer;

        public List<T> Items
        {
            get
            {
                return _items;
            }
        }

        public ObservableSortedList()
            : this(new List<T>())
        {
        }

        public ObservableSortedList(IComparer<T> comparer)
            : this(new List<T>(), comparer)
        {
        }

        public ObservableSortedList(List<T> collection)
        {
            _items = collection;
        }

        public ObservableSortedList(List<T> collection, IComparer<T> comparer)
            : this(collection)
        {
            _comparer = comparer;
            _items.Sort(_comparer);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            item.PropertyChanged += ItemOnPropertyChanged;
            List<T> changed = new List<T> { item };
            if (_comparer != null)
            {
                int index = 0;
                if (_items.Count > 0)
                {
                    for (int i = 0; i < _items.Count; i++)
                    {
                        if (_comparer.Compare(item, _items[i]) >= 0)
                            break;
                        index = i;
                    }
                    changed.Add(_items[index]);
                }
                _items.Insert(index, item);
            }
            else
            {
                _items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changed));
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is T)
            {
                T item = (T)sender;

                int indexOf = _items.IndexOf(item);

//                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item, indexOf));
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            IEnumerable<T> items = collection as T[] ?? collection.ToArray();
            foreach (T item in items)
            {
                item.PropertyChanged += ItemOnPropertyChanged;
            }
            _items.AddRange(items);

            if (_comparer != null)
            {
                _items.Sort(_comparer);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            _items.ForEach(x => x.PropertyChanged -= ItemOnPropertyChanged);
            _items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool result = _items.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return result;
        }

        public int Count { get { return _items.Count; } }
        public bool IsReadOnly { get { return false; } }
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            T item = _items[index];
            item.PropertyChanged -= ItemOnPropertyChanged;
            _items.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null) CollectionChanged(this, e);
        }
    }
}
