/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace ZoomCloser.Utils
{
    public static class WindowFindExtentiion
    {
        public static HWND Find(this HWND parent, HWND childAfter, string className = null, string windowTitle = null)
            => User32.FindWindowEx(parent, childAfter, className, windowTitle);
        public static HWND Find(this HWND parent, string className = null, string windowTitle = null)
            => User32.FindWindowEx(parent, IntPtr.Zero, className, windowTitle);

        public static bool TryFind(this HWND parent, out HWND child, string className = null, string windowTitle = null)
        {
            child = parent.Find(className, windowTitle);
            return !child.IsNull;
        }

        public static bool TryFind(this HWND parent, HWND childAfter, out HWND child, string className = null, string windowTitle = null)
        {
            child = parent.Find(childAfter, className, windowTitle);
            return !child.IsNull;
        }

        public static IEnumerable<HWND> FindMany(this HWND parent, string className = null, string windowTitle = null)
        {
            HWND childAfter = IntPtr.Zero;
            while (true)
            {
                if (TryFind(parent, childAfter, out HWND child, className, windowTitle))
                {
                    yield return child;
                    childAfter = child;
                }
                else
                {
                    yield break;
                }
            }
        }

        public static HWND Find(string className = null, string windowTitle = null) => User32.FindWindow(className, windowTitle);
        public static bool TryFind(out HWND child, string className = null, string windowTitle = null)
        {
            child = Find(className, windowTitle);
            return !child.IsNull;
        }

        public static IEnumerable<HWND> FindMany(string className = null, string windowTitle = null)
            => FindMany(IntPtr.Zero, className, windowTitle);

        public static IEnumerable<HWND> FindAll(this HWND parent)
        {
            var found = FindMany(parent);
            foreach (var s in found)
            {
                yield return s;
                foreach (var s2 in FindAll(s))
                {
                    yield return s2;
                }
            }
        }
    }
}
