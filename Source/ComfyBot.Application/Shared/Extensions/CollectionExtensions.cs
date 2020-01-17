namespace ComfyBot.Application.Shared.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Registers the specified <paramref name="action"/> to be invoked on each element
        /// that is added to the <paramref name="collection"/> or already exists in it.
        /// Automatically removes the action from an element when it is removed from the collection.
        /// </summary>
        public static void RegisterCollectionItemChanged<T>(this ObservableCollection<T> collection,
                                                            Action<object, PropertyChangedEventArgs> action)
                                              where T: INotifyPropertyChanged
        {
            void HandleCollectionEvent(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    foreach (T newItem in e.NewItems)
                    {
                        newItem.PropertyChanged += new PropertyChangedEventHandler(action);
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (T oldItem in e.OldItems)
                    {
                        oldItem.PropertyChanged -= new PropertyChangedEventHandler(action);
                    }
                }
            }

            foreach (T item in collection)
            {
                item.PropertyChanged += new PropertyChangedEventHandler(action);
            }

            collection.CollectionChanged += HandleCollectionEvent;
        }

        public static IEnumerable<TextModel> ToTextModels(this IEnumerable<string> collection)
        {
            return collection.Select(s => new TextModel { Text = s });
        }
    }
}