using CoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoomCloser.Utils;

namespace ZoomCloser.Services.Audio
{
    public class AudioService : IAudioService
    {
        public AudioService(MMDeviceEnumerator devEnum)
        {
            this.devEnum = devEnum;
        }

        private readonly MMDeviceEnumerator devEnum;

        public void SetMute(bool mute)
        {
            MMDevice device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            foreach (var session in device.AudioSessionManager2.Sessions)
            {
                Process p = Process.GetProcessById((int)session.GetProcessID);
                string name = p.ProcessName;
                if (Regex.IsMatch(name, "Zoom"))
                {
                    SimpleAudioVolume vol = session.SimpleAudioVolume;
                    vol.Mute = mute;
                }
            }
        }
    }
}
