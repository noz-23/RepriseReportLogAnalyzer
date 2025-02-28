using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Extensions
{   static class CollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> src_, IEnumerable<T> items_)
        {
            foreach (var item in items_)
            {
                src_.Add(item);
            }
        }
    }
}
