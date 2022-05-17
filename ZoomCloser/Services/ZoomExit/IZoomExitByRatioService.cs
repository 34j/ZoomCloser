using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    /// <summary>
    /// Represents a service that automatically exit the Zoom Meeting according to <see cref="IReadOnlyZoomHandlingService"/> and <see cref="IJudgingWhetherToExitByRatioService"/>. (using timer)
    /// </summary>
    public interface IZoomExitByRatioService : IZoomExitService
    {
        /// <summary>
        /// The service this service uses to judge whether to exit the meeting.
        /// </summary>
        IJudgingWhetherToExitByRatioService JudgingWhetherToExitByRatioService { get; }
    }
}
