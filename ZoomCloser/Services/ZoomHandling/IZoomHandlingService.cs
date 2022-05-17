using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <returns></returns>
        Task<bool> Exit();
        /// <summary>
        /// Refreshes <see cref="IReadOnlyZoomHandlingService.ParticipantCount"/>.
        /// </summary>
        void RefreshParticipantCount();
    }
}
