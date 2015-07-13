using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace ChordsKaraoke.Data.ViewModels
{
    public class ObservableSortedList<T> : ICollection<T>, INotifyCollectionChanged
        where T : INotifyPropertyChanged
    {
        private readonly IComparer<T> _comparer;
        private readonly List<T> _items;
        private readonly Mutex _mutex = new Mutex();

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

        public T this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException(); }
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
            var changed = new List<T> {item};
            item.PropertyChanged += OnCollectionItemChanged;
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changed,
                _items.IndexOf(item)));
        }

        public void Clear()
        {
            _items.ForEach(x => { x.PropertyChanged -= OnCollectionItemChanged; });
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
            int index = IndexOf(item);
            RemoveAt(index);
            return true;
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

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
            item.PropertyChanged -= OnCollectionItemChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            _items.RemoveAt(index);
        }

        private void OnCollectionItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is T)
            {
                OnItemChanged((T) sender, e.PropertyName);
            }
        }


        public void AddRange(IEnumerable<T> collection)
        {
            T[] items = collection as T[] ?? collection.ToArray();
            foreach (T item in items)
            {
                item.PropertyChanged += OnCollectionItemChanged;
            }

            _items.AddRange(items);

            if (_comparer != null)
            {
                _items.Sort(_comparer);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _mutex.WaitOne();
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null) handler(this, e);
            _mutex.ReleaseMutex();
        }

        public event PropertyChangedEventHandler CollectionItemChanged;

        protected virtual void OnItemChanged(T item, string property)
        {
            if (CollectionItemChanged != null)
            {
                CollectionItemChanged(item, new PropertyChangedEventArgs(property));
            }
        }

        public void Sort()
        {
            if (_comparer != null)
            {
                _items.Sort(_comparer);
            }
        }
    }
}