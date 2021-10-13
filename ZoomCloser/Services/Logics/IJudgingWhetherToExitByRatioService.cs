using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    public interface IJudgingWhetherToExitByRatioService : IJudgingWhetherToExitService
    {
        #region Ratio
        double MaximumRatioOfCurrentCountToMaxCountToExit { get; }
        int MaximumCountToExit { get; }
        int MaximumCount { get; }
        #endregion Ratio

        #region Threshold
        int ThresholdToActivation { get; }
        bool IsOverThresholdToActivation { get; }
        #endregion Threshold

        int CurrentCount { get; }
    }
}
