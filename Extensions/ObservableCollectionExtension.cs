using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RLMLogReader.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> src_, ICollection<T>collection_)
        {
            collection_?.ToList().ForEach(item => src_.Add(item));
        }
    }
}
