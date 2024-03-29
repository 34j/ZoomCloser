﻿/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vanara.PInvoke;
using WindowsInput;
using WindowsInput.Events;

namespace ZoomCloser.Services.ZoomHandling
{
    /// <summary>
    /// A service that handles the window of Zoom Meeting.  
    /// This service will automatically operate the window of Zoom Meeting so that 
    /// the number of participants can be always taken when <see cref="ZoomHandlingService.ZoomState"/> getter is called.
    /// </summary>
    public class ZoomActivatingHandlingService : ZoomHandlingService
    {
        public ZoomActivatingHandlingService()
        {
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
                            User32.ShowWindow(Handle, ShowWindowCommand.SW_SHOWMAXIMIZED);
                            Debug.WriteLine("Window too small, maximize it.");
                            break;
                    }
                }
            };
        }

        private async Task SimulateKeys(params KeyCode[] keys)
        {
            User32.SetFocus(Handle);
            await Simulate.Events().ClickChord(keys).Invoke();
        }

        private IntPtr Handle => new(MainWindowElement.Current.NativeWindowHandle);
    }
}
