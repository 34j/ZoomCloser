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
    public class ZoomExitByRatioService : ZoomExitService, IZoomExitByRatioService
    {
        public IJudgingWhetherToExitByRatioService JudgingWhetherToExitByRatioService { get; }

        public ZoomExitByRatioService(IZoomHandlingService zoomHandlingService, IJudgingWhetherToExitByRatioService judgingWhetherToExitByRatioService, Timer timer) : base(zoomHandlingService, judgingWhetherToExitByRatioService, timer)
        {
            this.JudgingWhetherToExitByRatioService = judgingWhetherToExitByRatioService;
        }
    }
}