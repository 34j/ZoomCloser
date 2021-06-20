/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace ZoomCloser.Utils
{
    public static class MyUser32Extention
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);


        public static string GetWindowText(this HWND hWND)
        {
            var size = User32.SendMessage(hWND, WindowMessage.WM_GETTEXTLENGTH);
            StringBuilder sb = new StringBuilder((int)size);
            SendMessage((IntPtr)hWND, (uint)WindowMessage.WM_GETTEXT, (int)size, sb);
            string text = sb.ToString();
            return text;
        }

        public static string GetWindowTitle(this HWND hWND)
        {
            int length = GetWindowTextLength(hWND);
            StringBuilder sb = new StringBuilder(length);
            User32.GetWindowText(hWND, sb, length);
            return sb.ToString();
        }


        public static bool IsWindowAvailable(this HWND hWND)
        {
            if (hWND == IntPtr.Zero || !User32.IsWindow(hWND))
            {
                return false;
            }
            return true;
        }



    }

    public static class WindowFindExtentiion
    {
        public static HWND Find(this HWND parent, HWND childAfter, string className = null, string windowTitle = null)
            => User32.FindWindowEx(parent, childAfter, className, windowTitle);
        public static HWND Find(this HWND parent, string className = null, string windowTitle = null)
            => User32.FindWindowEx(parent, IntPtr.Zero, className, windowTitle);

        public static bool TryFind(this HWND parent, out HWND child, string className = null, string windowTitle = null)
        {
            child = parent.Find(className, windowTitle);
            if (child.IsNull) return false;
            return true;
        }

        public static bool TryFind(this HWND parent, HWND childAfter, out HWND child, string className = null, string windowTitle = null)
        {
            child = parent.Find(childAfter, className, windowTitle);
            if (child.IsNull) return false;
            return true;
        }

        public static IEnumerable<HWND> FindMany(this HWND parent, string className = null, string windowTitle = null)
        {
            HWND childAfter = IntPtr.Zero;
            while (true)
            {
                if(TryFind(parent, childAfter, out HWND child, className, windowTitle))
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
            if (child.IsNull) return false;
            return true;
        }

        public static IEnumerable<HWND> FindMany(string className = null, string windowTitle = null)
            => FindMany(IntPtr.Zero, className, windowTitle);

    }

    public static class LinqExtention
    {

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
