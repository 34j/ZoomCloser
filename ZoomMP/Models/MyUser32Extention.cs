using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace ZoomCloserJp.Models
{
    public static class MyUser32Extention
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);


        public static string GetControlText(HWND hWND)
        {
            var size = User32.SendMessage(hWND, WindowMessage.WM_GETTEXTLENGTH);
            StringBuilder sb = new StringBuilder((int)size);
            SendMessage((IntPtr)hWND, (uint)WindowMessage.WM_GETTEXT, (int)size, sb);
            string text = sb.ToString();
            return text;
        }

        public static string GetWindowText_(HWND hWND)
        {
            int length = GetWindowTextLength(hWND);
            StringBuilder sb = new StringBuilder(length);
            GetWindowText(hWND, sb, length);
            return sb.ToString();
        }

        /// <summary>
        /// 失敗
        /// </summary>
        /// <param name="keys"></param>
        /*public static void SendInputKeyPressAndRelease(byte[] keys)
        {
            int length = keys.Length;
            int arraySize = length * 2;
            INPUT[] inputs = new INPUT[arraySize];
            for (int i = 0; i < length; i++)
            {
                inputs[i].type = INPUTTYPE.INPUT_KEYBOARD;
                inputs[i].ki.wScan = keys[i];
            }
            for (int i = 0; i < length; i++)
            {
                var index = i + length;
                inputs[index].type = INPUTTYPE.INPUT_KEYBOARD;
                inputs[index].ki.wScan = keys[i];
                inputs[index].ki.dwFlags = KEYEVENTF.KEYEVENTF_KEYUP;
            }
            int size;
            unsafe
            {
                size = sizeof(INPUT);
            }
            SendInput((uint)arraySize, inputs, size);

        }*/
    }
}
