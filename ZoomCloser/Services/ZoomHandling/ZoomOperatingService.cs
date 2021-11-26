using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsInput;
using WindowsInput.Events;

namespace ZoomCloser.Services.ZoomHandling
{
    public class ZoomOperatingService : ZoomHandlingService2
    {
        public ZoomOperatingService()
        {
            Debug.WriteLine("hello");
            this.PropertyChanged += async (_, e) =>
            {
                if (e.PropertyName == nameof(ZoomState))
                {
                    switch (ZoomState)
                    {
                        case ZoomErrorState.Minimized:
                            break;
                        case ZoomErrorState.MeetingControlNotAlwaysDisplayed:
                            await SimulateKeys(KeyCode.Alt);
                            break;
                        case ZoomErrorState.WindowTooSmall:
                            User32.ShowWindow(handle, ShowWindowCommand.SW_SHOWMAXIMIZED);
                            break;
                    }
                }
            };
        }

        private async Task SimulateKeys(params KeyCode[] keys)
        {
            User32.SetFocus(handle);
            await Simulate.Events().ClickChord(keys).Invoke();
        }

        private IntPtr handle => new IntPtr(MainWindowElement.Current.NativeWindowHandle);
    }
}
