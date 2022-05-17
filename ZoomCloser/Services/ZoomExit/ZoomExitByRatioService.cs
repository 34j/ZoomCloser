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
    /// A service that automatically exit the Zoom Meeting according to <see cref="IReadOnlyZoomHandlingService"/> and <see cref="IJudgingWhetherToExitByRatioService"/> using <see cref="Timer"/>.
    /// </summary>
    public class ZoomExitByRatioService : ZoomExitService, IZoomExitByRatioService
    {
        public IJudgingWhetherToExitByRatioService JudgingWhetherToExitByRatioService { get; }

        public ZoomExitByRatioService(ZoomOperatingService zoomHandlingService, IJudgingWhetherToExitByRatioService judgingWhetherToExitByRatioService, Timer timer) : base(zoomHandlingService, judgingWhetherToExitByRatioService, timer)
        {
            this.JudgingWhetherToExitByRatioService = judgingWhetherToExitByRatioService;
        }
    }
}