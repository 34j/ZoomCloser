using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using NAudio.CoreAudioApi;
using static ZoomCloser.Utils.LinqExtention;

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

        [Obsolete]
        public static void Debug()
        {
            MMDeviceEnumerator mmde = new MMDeviceEnumerator();
            MMDeviceCollection mmdc = mmde.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All);
            mmdc?.DebugIEnumerale(s =>
            {
                string result = s?.FriendlyName;
                try { result += s.AudioEndpointVolume; }
                catch (Exception e) { result += "error"; }
                return result;
            });
        }
    }
}
