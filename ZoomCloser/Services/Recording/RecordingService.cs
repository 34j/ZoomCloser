/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using ScreenRecorderLib;
using System;
using System.IO;
using System.Reflection;
using ZoomCloser.Services.ZoomWindow;
using System.Collections.Generic;
using ZoomCloser.Services.Settings;

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
            var source = new WindowRecordingSource(windowHandle);
            if (source.RecorderApi != RecorderApi.WindowsGraphicsCapture)
            {
                throw new NotSupportedException("Only Windows Graphics Capture is supported");
            }
            
            var options = new RecorderOptions()
            {
                AudioOptions = new AudioOptions()
                {
                    IsAudioEnabled = true,
                },
                VideoEncoderOptions = new VideoEncoderOptions()
                {
                    Bitrate = BasicSettings.Instance.BitRate,
                },
                MouseOptions = new MouseOptions()
                {
                    IsMousePointerEnabled = false,
                }
                ,
                SourceOptions = new SourceOptions()
                {
                    RecordingSources = new List<RecordingSourceBase>()
                    {
                        source
                    }
                }
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
