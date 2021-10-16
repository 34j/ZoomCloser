using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using static ZoomCloser.Utils.LinqExtention;
using CoreAudio;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
