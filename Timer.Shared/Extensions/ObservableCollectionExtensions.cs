﻿using System.Collections.ObjectModel;

namespace Timer.Shared.Extensions
{
    internal static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                observableCollection.Add(item);
            }
        }
    }
}
