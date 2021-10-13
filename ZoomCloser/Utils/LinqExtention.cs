using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Utils
{
    public static class LinqExtention
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static void DebugIEnumerale<T>(this IEnumerable<T> enumeration, Func<T, string> func)
        {
            StringBuilder sb = new StringBuilder();
            enumeration.ForEach(s =>
            {
                sb.Append(func(s));
                sb.Append(", ");
            });
            Debug.WriteLine(sb.ToString());
        }
    }
}
