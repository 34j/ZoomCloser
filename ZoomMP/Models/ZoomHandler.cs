using System;
using System.Text.RegularExpressions;
using Vanara.PInvoke;
using WindowsInput;
using WindowsInput.Events;
using static Vanara.PInvoke.User32;

namespace ZoomCloserJp.Models
{
    class ZoomHandler
    {
        public enum ZoomMode { E_NotRunning, E_NoMemberList, E_DifferentShortCuts, OK }
        public ZoomMode ZoomMode_ { get; set; } = ZoomMode.E_NotRunning;
        HWND contentRightPanelWH;
        HWND zRightPanelContainerClassWH;
        HWND zPlistWndClassWH;
        HWND parentWH;
        //HWND contentLeftPanelHWnd;
        //HWND zPControlPanelClassHWnd;
        //HWND contentRightPanelHWnd;
        int errorCount = 0;

        public int ParticipantNumbers { get; set; } = 0;


        public async void CloseZoom()
        {
            SetForegroundWindow(parentWH);
            SetFocus(parentWH);
            await Simulate.Events().ClickChord(KeyCode.Menu, KeyCode.Q).Wait(200).ClickChord(KeyCode.Return).Invoke();
            DestroyWindow(parentWH);
        }

        public int? GetParticipantNumbers()
        {
            if (zPlistWndClassWH.IsNull)
            {                
                if (GetWHs() == false)
                {
                    return null;
                }
            }
            var text = MyUser32Extention.GetControlText(zPlistWndClassWH);
            if (text == "")
            {
                ZoomMode_ = ZoomMode.E_NotRunning;
                return null;
            }
            string numStr = Regex.Match(text, @"\d+").ToString();
            int num = int.Parse(numStr);
            ParticipantNumbers = num;
            ZoomMode_ = ZoomMode.OK;
            return num;
        }

        public bool GetWHs()
        {
            parentWH = FindWindow("ZPContentViewWndClass", "Zoom ミーティング");
            if (parentWH.IsNull)
            {
                ZoomMode_ = ZoomMode.E_NotRunning;
                return false;
            }
            contentRightPanelWH = FindWindowEx(parentWH, IntPtr.Zero, null, "ContentRightPanel");
            if (contentRightPanelWH.IsNull)
            {
                ZoomMode_ = ZoomMode.E_NotRunning;
                return false;
            }
            zRightPanelContainerClassWH = FindWindowEx(contentRightPanelWH, IntPtr.Zero, "zRightPanelContainerClass", null);
            if (zRightPanelContainerClassWH.IsNull)
            {
                ZoomMode_ = ZoomMode.E_NotRunning;
                return false;
            }
            zPlistWndClassWH = FindWindowEx(zRightPanelContainerClassWH, IntPtr.Zero, null, null);
            // contentLeftPanelHWnd = FindWindowEx(parentWH, IntPtr.Zero, null, "ContentLeftPanel");
            // zPControlPanelClassHWnd = FindWindowEx(contentLeftPanelHWnd, IntPtr.Zero, "ZPControlPanelClass", null);
            // contentRightPanelHWnd = FindWindowEx(parentWH, IntPtr.Zero, null, "ContentRightPanel");


            if (zPlistWndClassWH.IsNull)
            {
                SetFocus(parentWH);
                //inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_U);
                //zPlistWndClassWH = FindWindowEx(zRightPanelContainerClassWH, IntPtr.Zero, null, null);
                if (zPlistWndClassWH.IsNull)
                {
                    if (errorCount > 50)
                    {
                        errorCount = 0;
                        parentWH = new HWND();
                    }
                    ZoomMode_ = ZoomMode.E_NoMemberList;
                    errorCount++;
                    return false;
                }
            }
            return true;
        }


    }
}
