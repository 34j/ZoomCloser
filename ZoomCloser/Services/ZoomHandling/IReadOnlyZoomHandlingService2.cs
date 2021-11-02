using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services.ZoomHandling
{
    public interface IReadOnlyZoomHandlingService2
    {
        ZoomState ZoomState { get; }

        int? ParticipantCount { get; }

        event EventHandler OnEntered;
        event EventHandler OnParticipantCountAvailable;
        event EventHandler OnExit;
        event EventHandler OnThisForcedExit;
        event EventHandler OnNotThisForcedExit;
    }
}
