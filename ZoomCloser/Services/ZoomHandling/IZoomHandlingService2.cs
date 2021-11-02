using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Services.ZoomHandling
{
    public interface IZoomHandlingService2 : IReadOnlyZoomHandlingService2
    {
        Task<bool> Exit();

        void RefreshParticipantCount();
    }
}
