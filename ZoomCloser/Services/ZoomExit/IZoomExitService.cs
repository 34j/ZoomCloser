using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ZoomCloser.Services.ZoomHandling;

namespace ZoomCloser.Services
{
    /// <summary>
    /// Represents a service that automatically exit the Zoom Meeting according to <see cref="IReadOnlyZoomHandlingService"/> and <see cref="IJudgingWhetherToExitService"/>. (using timer)
    /// </summary>
    public interface IZoomExitService
    {
        /// <summary>
        /// Read only.
        /// </summary>
        IReadOnlyZoomHandlingService ReadOnlyZoomHandlingService { get; }

        /// <summary>
        /// Occurs when this service checked whether to exit the meeting.
        /// </summary>
        event EventHandler OnRefreshed;

        /// <summary>
        /// Manually exit the meeting.
        /// </summary>
        Task ExitManually();
        /// <summary>
        /// Whether this service can force to exit the meeting.
        /// </summary>
        bool IsActivated { get; set; }
    }
}
