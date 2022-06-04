/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        public static void DebugIEnumerable<T>(this IEnumerable<T> enumeration, Func<T, string> func)
        {
            StringBuilder sb = new();
            enumeration.ForEach(s =>
            {
                sb.Append(func(s));
                sb.Append(", ");
            });
            Debug.WriteLine(sb.ToString());
        }
    }
}
