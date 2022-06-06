/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace ZoomCloser.Utils
{
    internal static class WindowAudioExtention
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        public static void MuteWin32(this HWND hWND)
        {
            SendMessage(hWND, WindowMessage.WM_APPCOMMAND, hWND, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }
    }
}
