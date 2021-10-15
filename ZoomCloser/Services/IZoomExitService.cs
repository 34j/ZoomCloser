using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ZoomCloser.Services
{
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
    }
}
