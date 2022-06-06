/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Text;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace ZoomCloser.Utils
{
    public static class WindowExtention
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        public static string GetWindowText(this HWND hWND)
        {
            var size = User32.SendMessage(hWND, WindowMessage.WM_GETTEXTLENGTH);
            StringBuilder sb = new((int)size);
            SendMessage((IntPtr)hWND, (uint)WindowMessage.WM_GETTEXT, (int)size, sb);
            string text = sb.ToString();
            return text;
        }

        public static string GetWindowTitle(this HWND hWND)
        {
            int length = GetWindowTextLength(hWND) + 1;
            StringBuilder sb = new(length);
            if (User32.GetWindowText(hWND, sb, length) == 0)
                return null;
            return sb.ToString();
        }

        public static bool IsWindowAvailable(this HWND hWND)
        {
            return hWND != IntPtr.Zero && IsWindow(hWND);
        }
    }
}
