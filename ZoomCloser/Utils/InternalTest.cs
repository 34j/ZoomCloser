using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreAudio;
using NUnit;
using NUnit.Framework;

namespace ZoomCloser.Utils
{
    [TestFixture]
    public class InternalTest
    {
        [Test]
        public void AudioTest()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            foreach (var session in device.AudioSessionManager2.Sessions)
            {
                Process p = Process.GetProcessById((int)session.GetProcessID);
                string name = p.ProcessName;
                if (Regex.IsMatch(name, "Zoom"))
                {
                    SimpleAudioVolume vol = session.SimpleAudioVolume;
                    vol.Mute = !vol.Mute;
                }
            }
        }

    }
}
