using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    /// <summary>
    /// Mainly reliant on whether <see cref="participantCount"/> is less or equals to <see cref="MaximumCountToExit"/>.
    /// </summary>
    public interface IJudgingWhetherToExitByRatioService : IJudgingWhetherToExitService
    {
        #region Ratio
        /// <summary>
        /// Constant value that decides <see cref="P:MaximumCountToExit"/> / <see cref="P:MaximumCount"/>.
        /// </summary>
        double MaximumRatioOfCurrentCountToMaxCountToExit { get; }
        /// <summary>
        /// This is <see cref="P:MaximumCount"/> * <see cref="P:MaximumRatioOfCurrentCountToMaxCountToExit"/>.
        /// </summary>
        int MaximumCountToExit { get; }
        /// <summary>
        /// Highest value of <see cref="P:CurrentCount"/> since <see cref="IJudgingWhetherToExitService.Reset"/> was called.
        /// When <see cref="IJudgingWhetherToExitService.Reset"/> is called this value would be 0.
        /// </summary>
        int MaximumCount { get; }
        #endregion Ratio

        #region Threshold
        /// <summary>
        /// If <see cref="P:CurrentCount"/> is less than or equal to this value, <see cref="IJudgingWhetherToExitService.Judge(int)"/> always returns <c>false</c>.
        /// </summary>
        int ThresholdToActivation { get; }
        /// <summary>
        /// Whether <see cref="P:CurrentCount"/> is greater than <see cref="P:ThresholdToActivation"/>.
        /// </summary>
        bool IsOverThresholdToActivation { get; }
        #endregion Threshold
        /// <summary>
        /// The current number of participants in the meeting.
        /// </summary>
        int CurrentCount { get; }
    }
}
