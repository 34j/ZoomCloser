using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    public interface IReadOnlyZoomHandlingService
    {
        /// <summary>
        /// Current state of this service.
        /// </summary>
        ZoomHandlingServiceState ZoomMode { get; }

        /// <summary>
        /// Returns true if this service is working as intended.
        /// </summary>
        bool IsZoomModeError { get; }

        /// <summary>
        /// Number of the participants in a meeting. This value will not automatically be refreshed. To refresh the value, call RefreshParticipantCount().
        /// </summary>
        int ParticipantCount { get; }

        /// <summary>
        /// Occurs when entered a meeting.
        /// </summary>
        event EventHandler OnEntered;
        /// <summary>
        /// Occurs when this service successfully read the number of participants for the first time since entered the meeting.
        /// </summary>
        event EventHandler OnParticipantCountAvailable;
        /// <summary>
        /// Occurs when exit a meeting.
        /// </summary>
        event EventHandler OnExit;
        /// <summary>
        /// Occurs when this service successfully forced to exit a meeting. Occurs after OnExit occurred.
        /// </summary>
        event EventHandler OnThisForcedExit;
        /// <summary>
        /// Occurs when someone / something other than this service forced to exit a meeting. Occurs after OnExit occurred.
        /// </summary>
        event EventHandler OnNotThisForcedExit;
    }
}