using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    public interface IZoomExitByRatioService : IZoomExitService
    {
        IJudgingWhetherToExitByRatioService JudgingWhetherToExitByRatioService { get; }
    }
}
