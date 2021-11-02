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

namespace ZoomCloser.Services
{
    public enum ZoomHandlingServiceState { E_UnableToParse = -4, E_DifferentShortCuts = -3, E_NoMemberList = -2, E_NotRunning = -1, OK = 0 }


    public class ZoomHandlingService : IZoomHandlingService
    {
        public ZoomHandlingServiceState ZoomMode { get; private set; } = ZoomHandlingServiceState.E_NotRunning;
        public bool IsZoomModeError => ZoomMode == ZoomHandlingServiceState.OK;

        public event EventHandler OnEntered;
        public event EventHandler OnParticipantCountAvailable;
        public event EventHandler OnExit;
        public event EventHandler OnThisForcedExit;
        public event EventHandler OnNotThisForcedExit;

        private bool isLastExitWasForcedByThis = true;

        public int ParticipantCount { get; private set; } = 0;
        public bool IsParticipantCountAvailbale => zPlistWndClassWH.IsWindowAvailable();

        private HWND parentWH;
        private HWND contentRightPanelWH;
        private HWND zRightPanelContainerClassWH;
        private HWND zPlistWndClassWH;

        public ZoomHandlingService()
        {
        }

        public async Task<bool> Exit()
        {
            isLastExitWasForcedByThis = true;
            if (!parentWH.IsWindowAvailable())
            {
                return true;
            }
            User32.SetForegroundWindow(parentWH);
            User32.SetFocus(parentWH);
            await Simulate.Events().ClickChord(KeyCode.Menu, KeyCode.Q).Wait(200).ClickChord(KeyCode.Return).Invoke().ConfigureAwait(false);
            User32.DestroyWindow(parentWH);
            if (!parentWH.IsWindowAvailable())
            {
                ZoomMode = ZoomHandlingServiceState.E_DifferentShortCuts;
                return false;
            }
            OnThisForcedExit?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public async Task<(bool, int)> RefreshAndGetParticipantCount()
        {
            // parentWH.FindAll().DebugIEnumerale(s => s.GetWindowTitle());

            if (!CheckAndTryGetWindowHandles())
            {
                ParticipantCount = -1;
                return (false, ParticipantCount);
            }
            var text = zPlistWndClassWH.GetWindowText();
            var match = Regex.Match(text, @"\d+");
            if (int.TryParse(match.Value, out _))
            {
                ParticipantCount = int.Parse(match.Value);
                return (true, ParticipantCount);
            }
            else
            {
                ZoomMode = ZoomHandlingServiceState.E_UnableToParse;
                ParticipantCount = -1;
                return (false, ParticipantCount);
            }
        }

        public async Task<bool> RefreshParticipantCount()
        {
            return (await RefreshAndGetParticipantCount().ConfigureAwait(false)).Item1;
        }

        private bool CheckAndTryGetWindowHandles()
        {
            var lastZoomMode = ZoomMode;//値型
            if (!IsParticipantCountAvailbale)//window handle 
            {
                GetWindowHandles();
                if (ZoomMode == ZoomHandlingServiceState.OK)
                {
                    if (lastZoomMode == ZoomHandlingServiceState.E_NotRunning)
                    {
                        OnEntered?.Invoke(this, EventArgs.Empty);
                    }
                    OnParticipantCountAvailable?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else if (ZoomMode == ZoomHandlingServiceState.E_NotRunning)
                {
                    if (lastZoomMode != ZoomHandlingServiceState.E_NotRunning)
                    {
                        OnExit?.Invoke(this, EventArgs.Empty);
                        if (isLastExitWasForcedByThis)
                        {
                            isLastExitWasForcedByThis = false;
                        }
                        else
                        {
                            OnNotThisForcedExit?.Invoke(this, EventArgs.Empty);
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
                if (parents.Length == 0)
                {
                    break;
                }
                else
                {
                    parentWH = parents[0];
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
                ZoomMode = ZoomHandlingServiceState.E_NotRunning;
                return;
            }

            if (zPlistWndClassWH.IsNull)
            {
                Simulate2();
                ZoomMode = ZoomHandlingServiceState.E_NoMemberList;
                return;
            }
            ZoomMode = ZoomHandlingServiceState.OK;
            return;
        }

        private int hasSimulated = 0;
        private void Simulate2()
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
            Simulate.Events().MoveTo(x, y).Wait(100).DoubleClick(ButtonCode.Left).Wait(100).Hold(KeyCode.LAlt).Wait(30).Hold(KeyCode.U).Wait(100).Release(KeyCode.U).Release(KeyCode.LAlt).Invoke();
            Debug.WriteLine("simulated");
        }
    }

}
