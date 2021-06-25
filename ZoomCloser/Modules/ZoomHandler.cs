/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.Text.RegularExpressions;
using Vanara.PInvoke;
using WindowsInput;
using WindowsInput.Events;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using ZoomCloser.Utils;

namespace ZoomCloser.Modules
{
    class ZoomHandler
    {
        public enum ZoomMode { E_NotRunning, E_NoMemberList, E_DifferentShortCuts, OK, E_UnableToParse }
        public ZoomMode zoomMode { get; private set; } = ZoomMode.E_NotRunning;
        public enum ZoomModeState { Error, Warning, Ok }
        public ZoomModeState zoomModeState
        {
            get
            {
                switch (zoomMode.ToString()[0])
                {
                    case 'E':
                        return ZoomModeState.Error;
                    case 'W':
                        return ZoomModeState.Warning;
                    case 'O':
                        return ZoomModeState.Ok;
                }
                return ZoomModeState.Ok;
            }
        }

        public event EventHandler OnEntered;
        public event EventHandler OnParticipantCountAvailable;
        public event EventHandler OnExit;
        public event EventHandler OnThisForcedExit;
        public event EventHandler OnNotThisForcedExit;

        private bool isLastExitWasForcedByThis = true;

        public int ParticipantCount { get; private set; } = 0;
        public bool IsParticipantCountAvailbale => zPlistWndClassWH.IsWindowAvailable();


        HWND parentWH;
        HWND contentRightPanelWH;
        HWND zRightPanelContainerClassWH;
        HWND zPlistWndClassWH;

        public async Task<bool> CloseZoom()
        {
            isLastExitWasForcedByThis = true;
            if (!parentWH.IsWindowAvailable())
            {
                return true;
            }
            User32.SetForegroundWindow(parentWH);
            User32.SetFocus(parentWH);
            await Simulate.Events().ClickChord(KeyCode.Menu, KeyCode.Q).Wait(200).ClickChord(KeyCode.Return).Invoke();
            User32.DestroyWindow(parentWH);
            if (!parentWH.IsWindowAvailable())
            {
                zoomMode = ZoomMode.E_DifferentShortCuts;
                return false;
            }
            OnThisForcedExit?.Invoke(this, new EventArgs());
            return true;
        }

        public async Task<bool> GetParticipantCount()
        {
           // parentWH.FindAll().DebugIEnumerale(s => s.GetWindowTitle());

            if (!CheckAndTryGetWindowHandles())
            {
                return false;
            }
            var text = MyUser32Extention.GetWindowText(zPlistWndClassWH);
            var match = Regex.Match(text, @"\d+");
            if (int.TryParse(match.Value, out int result))
            {
                ParticipantCount = int.Parse(match.Value);
                return true;
            }
            else
            {
                zoomMode = ZoomMode.E_UnableToParse;
                return false;
            }
        }

        private bool CheckAndTryGetWindowHandles()
        {
            var lastZoomMode = zoomMode;//値型
            var lastZoomModeState = zoomModeState;
            if (!IsParticipantCountAvailbale)//window handle 
            {
                GetWindowHandles();
                if (zoomMode == ZoomMode.OK)
                {
                    if (lastZoomMode == ZoomMode.E_NotRunning)
                    {
                        OnEntered?.Invoke(this, new EventArgs());
                    }
                    OnParticipantCountAvailable?.Invoke(this, new EventArgs());
                    return true;
                }
                else if (zoomMode == ZoomMode.E_NotRunning)
                {
                    if (lastZoomMode != ZoomMode.E_NotRunning)
                    {
                        OnExit?.Invoke(this, new EventArgs());
                        if (isLastExitWasForcedByThis)
                        {
                            isLastExitWasForcedByThis = false;
                        }
                        else
                        {
                            OnNotThisForcedExit?.Invoke(this, new EventArgs());
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        private void GetWindowHandles()
        {
            bool flag = false;
            parentWH = IntPtr.Zero;
            contentRightPanelWH = IntPtr.Zero;
            zRightPanelContainerClassWH = IntPtr.Zero;
            zPlistWndClassWH = IntPtr.Zero;
            do
            {
                var parents = WindowFindExtentiion.FindMany("ZPContentViewWndClass").Where(s => s.GetWindowTitle().Length > 5).ToArray();
                if (parents.Count() == 0)
                {
                    break;
                }
                else
                {
                    parentWH = parents.First();
                }
                contentRightPanelWH = parentWH.Find(windowTitle: "ContentRightPanel");
                if (contentRightPanelWH.IsNull)
                {
                    break;
                }
                zRightPanelContainerClassWH = contentRightPanelWH.Find("zRightPanelContainerClass", "PlistContainer");
                if (zRightPanelContainerClassWH.IsNull)
                {
                    break;
                }
                zPlistWndClassWH = zRightPanelContainerClassWH.Find();
                flag = true;
            } while (false);
            if (!flag)
            {
                zoomMode = ZoomMode.E_NotRunning;
                return;
            }

            if (zPlistWndClassWH.IsNull)
            {
                Simulate2();
                zoomMode = ZoomMode.E_NoMemberList;
                return;
            }
            zoomMode = ZoomMode.OK;
            return;
        }

        int hasSimulated = 0;
        void Simulate2()
        {
            hasSimulated++;

            if (hasSimulated % 50 != 0)
            {
                return;
            }
            var ZPControlPanelClass = parentWH.Find(windowTitle: "ContentLeftPanel").Find("ZPControlPanelClass");
            User32.ShowWindow(parentWH, ShowWindowCommand.SW_MAXIMIZE);
            User32.SetFocus(ZPControlPanelClass);
            User32.GetWindowRect(ZPControlPanelClass, out RECT rect);
            int y = (rect.top + rect.bottom) / 2;
            int x = rect.right - 100;
            WindowsInput.Simulate.Events().MoveTo(x, y).Wait(100).DoubleClick(ButtonCode.Left).Wait(100).Hold(KeyCode.LAlt).Wait(30).Hold(KeyCode.U).Wait(100).Release(KeyCode.U).Release(KeyCode.LAlt).Invoke();
            Debug.WriteLine("simulated");
        }

    }
}
