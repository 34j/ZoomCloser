namespace ZoomCloser.Services.Recording
{
    public interface IRecordingService
    {
        bool IsRecording { get; }
        string FolderPath { get; }
        void StartRecording();
        void StopRecording();
    }
}
