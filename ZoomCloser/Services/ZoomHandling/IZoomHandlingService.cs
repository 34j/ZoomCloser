/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System.Threading.Tasks;

namespace ZoomCloser.Services.ZoomHandling
{
    /// <summary>
    /// Represents a service that handles the window of Zoom Meeting.
    /// </summary>
    public interface IZoomHandlingService : IReadOnlyZoomHandlingService
    {
        /// <summary>
        /// Exits the Zoom Meeting.
        /// </summary>
        /// <returns>Whether successufully exit the meeting.</returns>
        Task<bool> Exit();
        /// <summary>
        /// Refreshes <see cref="IReadOnlyZoomHandlingService.ParticipantCount"/>.
        /// </summary>
        /// <param name="refreshWindowIfWindowNotAvailable">Whether to refresh the window if the window is not available.</param>
        void RefreshParticipantCount(bool refreshWindowIfWindowNotAvailable = true);
    }
}
