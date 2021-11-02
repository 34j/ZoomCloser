using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace ZoomCloser.Services.ZoomWindow
{
    public static class ZoomWindowDetector
    {
        public static bool TryGetMainWindow(out IntPtr windowHandle, out bool isMinimized)
        {
            windowHandle = IntPtr.Zero;
            isMinimized = false;

            if (TryGetNormalMainWindow(out windowHandle))
            {
                return true;
            }
            else if(TryGetMinimizedMainWindow(out windowHandle))
            {
                isMinimized = true;
                return true;
            }

            return false;
        }

        private static bool TryGetNormalMainWindow(out IntPtr windowHandle)
        {
            //initialize
            windowHandle = IntPtr.Zero;

            //find normal window
            AutomationElement element = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ClassNameProperty, "ZPContentViewWndClass"));
            if (element != null)
            {
                windowHandle = new IntPtr(element.Current.NativeWindowHandle);
                return true;
            }

            //not found
            return false;
        }

        private static bool TryGetMinimizedMainWindow(out IntPtr windowHandle)
        {
            //initialize
            windowHandle = IntPtr.Zero;

            //find normal window
            AutomationElement element = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ClassNameProperty, "VideoFrameWndClass"));
            if (element != null)
            {
                windowHandle = new IntPtr(element.Current.NativeWindowHandle);
                return true;
            }

            //not found
            return false;
        }
    }
}