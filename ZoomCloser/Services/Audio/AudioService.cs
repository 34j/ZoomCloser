using CoreAudio;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZoomCloser.Utils;

namespace ZoomCloser.Services.Audio
{
    /// <summary>
    /// Mutes and unmutes the window of Zoom Meeting using CoreAudioApi.
    /// </summary>
    public class AudioService : IAudioService
    {

        /// <param name="devEnum">Default <see cref="MMDeviceEnumerator"/>. (Direct Injection)</param>
        public AudioService(MMDeviceEnumerator devEnum)
        {
            this.devEnum = devEnum;
        }

        private readonly MMDeviceEnumerator devEnum;

        private IEnumerable<AudioSessionControl2> GetSessions()
        {
            var result = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia)
                .AudioSessionManager2.Sessions/*EnumerateAudioEndPoints(EDataFlow.eAll, DEVICE_STATE.DEVICE_STATEMASK_ALL)
     .SelectMany(s => s?.AudioSessionManager2?.Sessions)*/
     .Where(session =>
     {
         string processName = Process.GetProcessById((int)session.GetProcessID).ProcessName;
         return processName.Contains("Zoom");
     });
            result.DebugIEnumerable(s => s.DisplayName);
            return result;
        }
        public void SetMute(bool mute)
        {
            GetSessions().ForEach(s => s.SimpleAudioVolume.Mute = mute);
        }

        public bool GetMute()
        {
            return GetSessions().Select(s => s.SimpleAudioVolume.Mute).Append(false).Aggregate((s, sum) => s || sum);
        }
    }
}
