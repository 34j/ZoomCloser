using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    public interface IZoomHandlingService : IReadOnlyZoomHandlingService
    {
        /// <summary>
        /// Refreshes and gets the number of participants.
        /// </summary>
        /// <returns>(whether it was successful, number of participants)</returns>
        Task<(bool, int)> RefreshAndGetParticipantCount();
        /// <summary>
        /// Gets the number of participants.
        /// </summary>
        /// <returns>Whether it was successful.</returns>
        Task<bool> RefreshParticipantCount();

        /// <summary>
        /// Exits a meeting.
        /// </summary>
        /// <returns>Whether it was successful.</returns>
        Task<bool> Exit();
    }
}