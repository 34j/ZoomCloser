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

namespace ZoomCloser.Services
{
    /// <summary>
    /// decides whether to close zoom
    /// </summary>
    public class ZoomExitService : IZoomExitService
    {
        private readonly Timer timer;

        private readonly IJudgingWhetherToExitService judgingWhetherToExitService;

        private readonly IZoomHandlingService zoomHandlingService;
        public IReadOnlyZoomHandlingService ReadOnlyZoomHandlingService => zoomHandlingService;

        public event EventHandler OnRefreshed;

        public ZoomExitService(IZoomHandlingService zoomHandlingService, IJudgingWhetherToExitService judgingWhetherToExitService, Timer timer)
        {
            this.zoomHandlingService = zoomHandlingService;
            this.judgingWhetherToExitService = judgingWhetherToExitService;
            this.timer = timer;
            timer.Interval = 100;
            timer.AutoReset = true;
            timer.Elapsed += async (sender, e) => await CheckAndClose().ConfigureAwait(false);
            timer.Elapsed += (_, e) => OnRefreshed?.Invoke(this, EventArgs.Empty);
            timer.Enabled = true;
        }

        public async Task ExitManually() => await zoomHandlingService.Exit().ConfigureAwait(false);
        private async Task CheckAndClose()
        {
            bool result = await zoomHandlingService.RefreshParticipantCount().ConfigureAwait(false);
            if (!result)
            {
                return;
            }
            bool shouldClose = judgingWhetherToExitService.Judge(zoomHandlingService.ParticipantCount);
            if (shouldClose)
            {
                judgingWhetherToExitService.Reset();
                await zoomHandlingService.Exit().ConfigureAwait(false);
            }
        }
    }
}

