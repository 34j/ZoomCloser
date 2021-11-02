using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;
using Vanara.PInvoke;
using WindowsInput;
using WindowsInput.Events;
using ZoomCloser.Services.ZoomWindow;

namespace ZoomCloser.Services.ZoomHandling
{

    public enum ZoomState { NotRunning, Normal, Minimized, NotExpectedBehaviour }
    public enum ZoomParticipantsState { }

    public class ZoomHandlingService2 : IZoomHandlingService2
    {
        public ZoomState ZoomState { get; private set; }



        public int? ParticipantCount { get; private set; }

        public event EventHandler OnEntered;
        public event EventHandler OnParticipantCountAvailable;
        public event EventHandler OnExit;
        public event EventHandler OnThisForcedExit;
        public event EventHandler OnNotThisForcedExit;

        public async Task<bool> Exit()
        {
            FindWindowElement();
            var handle = new IntPtr(mainWindowElement.Current.NativeWindowHandle);
            User32.SetForegroundWindow(handle);
            User32.SetFocus(handle);
            await Simulate.Events().ClickChord(KeyCode.Menu, KeyCode.Q).Wait(200).ClickChord(KeyCode.Return).Invoke().ConfigureAwait(false);
            //User32.DestroyWindow(parentWH);
            OnThisForcedExit?.Invoke(this, EventArgs.Empty);
            if (mainWindowElement.IsAlive())
            {
                return false;
            }
            return true;
        }


        public void RefreshParticipantCount()
        {
            if (!participantsElement.IsAlive())
            {
                FindParticipantsCountElement();
                return;
            }
            string name = "";
            try
            {
                name = participantsElement.Current.Name;
            }
            catch
            {
                return;
            }
            var match = Regex.Match(name, @"\d+");
            if (match != null && int.TryParse(match.Value, out int count))
            {
                ParticipantCount = count;
            }

        }

        private AutomationElement mainWindowElement;
        private AutomationElement participantsElement;


        void FindWindowElement()
        {
            //check if it is still alive
            if (mainWindowElement != null && !mainWindowElement.IsAlive())
            {
                mainWindowElement = null;
                OnExit?.Invoke(this, EventArgs.Empty);
            }

            //check if main window exists
            if (mainWindowElement == null)
            {
                if (ZoomWindowDetector.TryGetMainWindow(out IntPtr windowHandle, out bool isMinimized))
                {
                    mainWindowElement = AutomationElement.FromHandle(windowHandle);
                    if (isMinimized)
                    {
                        ZoomState = ZoomState.Minimized;
                    }
                    else
                    {
                        ZoomState = ZoomState.Normal;
                    }
                    OnEntered?.Invoke(this, new EventArgs());
                }
                else
                {
                    ZoomState = ZoomState.NotRunning;
                    return;
                }
            }
        }

        void FindParticipantsCountElement()
        {
            FindWindowElement();
            if (ZoomState == ZoomState.NotRunning || ZoomState == ZoomState.Minimized)
            {
                ParticipantCount = null;
                return;
            }

            //get button
            var controlPanelElement = mainWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "ZPControlPanelClass"));
            if (controlPanelElement == null)
            {
                //not expected
                ParticipantCount = null;
                ZoomState = ZoomState.NotExpectedBehaviour;
                //throw new NotImplementedException($"{nameof(controlPanelElement)} not found.");
                return;
            }

            var controlPanelChildElement = controlPanelElement.FindFirst(TreeScope.Children, Condition.TrueCondition);
            if (controlPanelChildElement == null)
            {
                //not expected
                ParticipantCount = null;
                ZoomState = ZoomState.NotExpectedBehaviour;
                //throw new NotImplementedException($"{nameof(controlPanelChildElement)} not found.");
                return;
            }

            var controlPanelButtonElements = controlPanelElement.FindAll(TreeScope.Descendants, Condition.TrueCondition);
            var buttonElement = controlPanelButtonElements.Cast<AutomationElement>().Where(s => Regex.IsMatch(s.Current.Name, @"\d"));
            if (buttonElement.Any())
            {
                ZoomState = ZoomState.Normal;
                participantsElement = buttonElement.First();
                return;
            }
            else
            {
            }




            var listContainer = mainWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "PListContainer"));
            if (listContainer == null)
            {
                ParticipantCount = null;
                return;
            }

            var listContainerChild = listContainer.FindAll(TreeScope.Children, new NotCondition(new PropertyCondition(AutomationElement.NameProperty, ""))).Cast<AutomationElement>().Where(s => Regex.IsMatch(s.Current.Name, @"\d"));
            if (listContainerChild.Any())
            {
                participantsElement = listContainerChild.First();
            }
            else
            {
                ParticipantCount = null;
                //not expected
                ZoomState = ZoomState.NotExpectedBehaviour;
                //throw new NotImplementedException($"{nameof(listContainerChild)} not found.");
                return;
            }

        }
    }
}
