/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Timers;
using ZoomCloser.Modules;
using ZoomCloser.Services.ZoomHandling;

namespace ZoomCloser.Services
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
        protected int TimeInterval { get; init; } = 100;

        private readonly IZoomHandlingService zoomHandlingService;
        public IReadOnlyZoomHandlingService ReadOnlyZoomHandlingService => zoomHandlingService;

        public event EventHandler OnRefreshed;
        
        public bool IsActivated { get; set; } = true;

        public T JudgingWhetherToExitService { get; }

        public ZoomMonitoringService(IZoomHandlingService zoomHandlingService, T judgingWhetherToExitService, Timer timer)
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
            zoomHandlingService.RefreshParticipantCount();

            int? count = zoomHandlingService.ParticipantCount;
            if (!count.HasValue)
            {
                return;
            }

            bool shouldClose = JudgingWhetherToExitService.Judge(count.Value);
            if (shouldClose)
            {
                if (!IsActivated)
                {
                    return;
                }
                JudgingWhetherToExitService.Reset();
                await zoomHandlingService.Exit().ConfigureAwait(false);
            }
        }
    }
}

