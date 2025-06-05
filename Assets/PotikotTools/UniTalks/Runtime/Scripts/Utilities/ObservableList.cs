using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PotikotTools.UniTalks
{
    public class ObservableList<T> : ObservableCollection<T>
    {
        public event Action<T> OnElementAdded;
        public event Action<T> OnElementRemoved;
        public event Action<int, T, T> OnElementChanged;
        public event Action OnCollectionCleared;

        public ObservableList()
        {
            Initialize();
        }

        public ObservableList(IEnumerable<T> collection) : base(collection)
        {
            Initialize();
        }

        public ObservableList(List<T> list) : base(list)
        {
            Initialize();
        }

        private void Initialize()
        {
            CollectionChanged += HandleCollectionChanged;
        }
        
        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                        OnElementAdded?.Invoke(item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                        OnElementRemoved?.Invoke(item);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++)
                        OnElementChanged?.Invoke(i, (T)e.OldItems[i], (T)e.NewItems[i]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnCollectionCleared?.Invoke();
                    break;
            }
        }
    }
}