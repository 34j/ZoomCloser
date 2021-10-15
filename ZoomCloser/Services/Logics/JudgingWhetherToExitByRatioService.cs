using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    public class JudgingWhetherToExitByRatioService : IJudgingWhetherToExitByRatioService
    {
        #region Ratio
        public double MaximumRatioOfCurrentCountToMaxCountToExit { get; } = 0.5f;
        public int MaximumCountToExit => (int)(MaximumRatioOfCurrentCountToMaxCountToExit * MaximumCount);
        public int MaximumCount { get; private set; } = 0;
        #endregion Ratio

        #region Threshold
        public int ThresholdToActivation { get; } = 3;
        public bool IsOverThresholdToActivation => CurrentCount > ThresholdToActivation;
        #endregion Threshold
        public int CurrentCount { get; private set; }

        public JudgingWhetherToExitByRatioService(double maximumRatioOfCurrentCountToMaxCountToExitInit = 0.5f)
        {
            const double error = 0.1;
            const double min = 0.1;
            const double max = 0.9;
            double ratio = maximumRatioOfCurrentCountToMaxCountToExitInit + RandomRange(-error, error);
            ratio = Math.Max(ratio, min);
            ratio = Math.Min(ratio, max);
            MaximumRatioOfCurrentCountToMaxCountToExit = ratio;
        }

        public bool Judge(int participantCount)
        {
            CurrentCount = participantCount;
            MaximumCount = Math.Max(participantCount, MaximumCount);

            return IsOverThresholdToActivation && participantCount <= MaximumCountToExit;
        }

        public void Reset()
        {
            CurrentCount = 0;
            MaximumCount = 0;
        }

        private static readonly Random random = new Random();
        private static double RandomRange(double min, double max)
        {
            double randDouble = random.NextDouble();
            double result = min + ((max - min) * randDouble);
            return result;
        }
    }
}
