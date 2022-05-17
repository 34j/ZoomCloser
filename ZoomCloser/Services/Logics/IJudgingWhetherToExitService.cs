using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services
{
    /// <summary>
    /// Represents a service that judges whether to exit a Zoom Meeting according to the number of participants.
    /// </summary>
    public interface IJudgingWhetherToExitService
    {
        /// <summary>
        /// Judge whether to exit the meeting. Call <see cref="Reset"></see> when successfully exit the meeting.
        /// </summary>
        /// <param name="participantCount">Current number of participants in the meeting.</param>
        /// <returns>Whether to exit the meeting or not.</returns>
        bool Judge(int participantCount);
        /// <summary>
        /// When successfully exit the meeting, you should call this function.
        /// </summary>
        void Reset();
    }
}
