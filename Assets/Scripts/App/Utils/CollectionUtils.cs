using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Utils
{
    public static class CollectionUtils
    {
        public static IEnumerable<T> PrintCollection<T>(this IEnumerable<T> collection, string separator = ",")
        {
            var printCollection = collection as T[] ?? collection.ToArray();
            
            var sb = new StringBuilder();
            foreach (var obj in printCollection)
            {
                sb.Append(obj);
                sb.Append(separator);
            }
            Debug.Log(sb.ToString());
            return printCollection;
        }
    }
}