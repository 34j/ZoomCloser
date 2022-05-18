/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using ZoomCloser.Services.ZoomHandling;

namespace ZoomCloser.Services.ZoomMonitoring
{
    /// <summary>
    /// A service that automatically exit the Zoom Meeting according to <see cref="IReadOnlyZoomHandlingService"/> and <see cref="IJudgingWhetherToExitService"/> using <see cref="Timer"/>.
    /// </summary>
    public class ZoomMonitoringService<T> : IZoomMonitoringService<T> where T : IJudgingWhetherToExitService
    {
        /// <summary>
        /// The <see cref="Timer"/> that will be used to judge whether to exit the Zoom Meeting.
        /// </summary>
        protected Timer CheckTimer { get; }
        public event EventHandler OnRefreshed;
        protected int TimeInterval { get; init; } = 100;
        protected int RefreshWindowIfWindowNotAvailableInterval = 5;
        private int refreshWindowIfWindowNotAvailableIntervalCounter = 0;

        private readonly IZoomHandlingService zoomHandlingService;
        public IReadOnlyZoomHandlingService ReadOnlyZoomHandlingService => zoomHandlingService;


        
        public bool AutoExit { get; set; } = true;
        public T JudgingWhetherToExitService { get; }

        public ZoomMonitoringService(ZoomActivatingHandlingService zoomHandlingService, T judgingWhetherToExitService, Timer timer)
        {
            this.zoomHandlingService = zoomHandlingService;
            this.JudgingWhetherToExitService = judgingWhetherToExitService;
            zoomHandlingService.OnExit += (_, e) => judgingWhetherToExitService.Reset();
            zoomHandlingService.OnEntered += (_, e) => judgingWhetherToExitService.Reset();

            this.CheckTimer = timer;
            timer.Interval = TimeInterval;
            timer.AutoReset = true;
            timer.Elapsed += async (sender, e) => await CheckAndClose().ConfigureAwait(false);
            timer.Elapsed += (_, e) => OnRefreshed?.Invoke(this, EventArgs.Empty);
            timer.Enabled = true;
        }

        public async Task ExitManually()
        {
            await zoomHandlingService.Exit().ConfigureAwait(false);
        }

        private async Task CheckAndClose()
        {
            refreshWindowIfWindowNotAvailableIntervalCounter = (refreshWindowIfWindowNotAvailableIntervalCounter + 1) % RefreshWindowIfWindowNotAvailableInterval;
            var refreshWindowIfWindowNotAvailable = refreshWindowIfWindowNotAvailableIntervalCounter == 0;
            zoomHandlingService.RefreshParticipantCount(refreshWindowIfWindowNotAvailable);

            int? count = zoomHandlingService.ParticipantCount;
            if (!count.HasValue)
            {
                return;
            }

            bool shouldClose = JudgingWhetherToExitService.Judge(count.Value);
            if (shouldClose)
            {
                if (!AutoExit)
                {
                    return;
                }
                JudgingWhetherToExitService.Reset();
                await zoomHandlingService.Exit().ConfigureAwait(false);
            }
        }
    }
}

