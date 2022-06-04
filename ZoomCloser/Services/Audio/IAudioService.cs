/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
namespace ZoomCloser.Services.Audio
{
    /// <summary>
    /// Represents a service that mute and unmute the Zoom Meetings window.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Mute or unmute the Zoom Meetings window.
        /// </summary>
        /// <param name="mute"></param>
        void SetMute(bool mute);
        /// <summary>
        /// Gets the current mute state of the Zoom Meetings window.
        /// </summary>
        /// <returns></returns>
        bool GetMute();
    }
}