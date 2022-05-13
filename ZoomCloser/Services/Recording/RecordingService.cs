using ScreenRecorderLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ZoomCloser.Services.ZoomWindow;

namespace ZoomCloser.Services.Recording
{
    
    public class RecordingService : IRecordingService
    {
        public string FolderPath => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        Recorder _rec;
        public bool IsRecording { get; private set; } = false;
        public void StartRecording()
        {
            if (IsRecording)
            {
                return;
            }

            if(!ZoomWindowDetector.TryGetMainWindow(out IntPtr windowHandle, out bool isMinimized))
            {
                return;
            }

            IsRecording = true;
            var options = new RecorderOptions()
            {
                AudioOptions = new AudioOptions()
                {
                    IsAudioEnabled = true,
                },
                VideoOptions = new VideoOptions()
                {
                    Bitrate = BasicSettings.Instance.BitRate,
                },
                MouseOptions = new MouseOptions()
                {
                    IsMousePointerEnabled = false,
                }
                ,
                DisplayOptions = new DisplayOptions()
                {
                    WindowHandle = windowHandle,
                },
                RecorderApi = RecorderApi.WindowsGraphicsCapture,
            };
            _rec = Recorder.CreateRecorder(options);
            string path = Path.Combine(FolderPath, DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".mp4");
            _rec.Record(path);
        }

        public void StopRecording()
        {
            if (IsRecording)
            {
                _rec.Stop();
                IsRecording = false;
            }
        }
    }
}
