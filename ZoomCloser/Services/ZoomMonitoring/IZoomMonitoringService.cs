/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Threading.Tasks;
using ZoomCloser.Services.ZoomHandling;

namespace ZoomCloser.Services.ZoomMonitoring
{
    /// <summary>
    /// Represents a service that automatically exit the Zoom Meeting according to <see cref="IReadOnlyZoomHandlingService"/> and <see cref="IJudgingWhetherToExitService"/>. (using timer)
    /// </summary>
    public interface IZoomMonitoringService<T> where T : IJudgingWhetherToExitService
    {
        /// <summary>
        /// 
        /// </summary>
        T JudgingWhetherToExitService { get; }
        /// <summary>
        /// Readonly <see cref="IReadOnlyZoomHandlingService"></see>
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
        bool AutoExit { get; set; }
    }
}
