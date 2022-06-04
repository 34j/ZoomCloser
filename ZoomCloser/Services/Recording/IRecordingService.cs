/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
namespace ZoomCloser.Services.Recording
{
    /// <summary>
    /// The service for recording the window of Zoom Meeting.
    /// </summary>
    public interface IRecordingService
    {
        /// <summary>
        /// Whether the recording is started.
        /// </summary>
        bool IsRecording { get; }
        /// <summary>
        /// The recording file path.
        /// </summary>
        string FolderPath { get; }
        /// <summary>
        /// Start recording.
        /// </summary>
        void StartRecording();
        /// <summary>
        /// Stop recording.
        /// </summary>
        void StopRecording();
    }
}
