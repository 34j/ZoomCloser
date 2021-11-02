namespace ZoomCloser.Services.Audio
{
    public interface IAudioService
    {
        /// <summary>
        /// Mute / UnMute zoom in volume mixer using CoreAudioApi.
        /// </summary>
        /// <param name="mute"></param>
        void SetMute(bool mute);

        bool GetMute();
    }
}