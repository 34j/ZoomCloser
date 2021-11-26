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

    public enum ZoomState { NotRunning, Normal, Minimized, WindowTooSmall, NotExpectedBehaviour }
    public enum ZoomParticipantsState { }

    public class ZoomHandlingService2 : IZoomHandlingService2
    {
        public ZoomState ZoomState { get; private set; }

        public bool? IsBreakoutRoom { get; private set; }
        public int? ParticipantCount { get; private set; }

        public event EventHandler OnEntered;
        public event EventHandler OnParticipantCountAvailable;
        public event EventHandler OnExit;
        public event EventHandler OnThisForcedExit;
        public event EventHandler OnNotThisForcedExit;

        private bool thisForcedExit = false;

        private AutomationElement mainWindowElement;
        private AutomationElement participantsElement;

        public ZoomHandlingService2()
        {
            //set IsBreakoutRoom.
            this.OnEntered += (_, e) => {
                var title = mainWindowElement.Current.Name;
                IsBreakoutRoom = Regex.IsMatch(title, @"\d+");
            };
            this.OnExit += (_, e) => {
                IsBreakoutRoom = null;
            };

            //set OnNotThisForcedExit.
            this.OnThisForcedExit += (_, e) =>
            {
                thisForcedExit = true;
            };

            this.OnExit += (_, e) =>
            {
                if (thisForcedExit)
                {
                    thisForcedExit = false;
                }
                else
                {
                    OnNotThisForcedExit?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public async Task<bool> Exit()
        {
            FindWindowElement();
            IntPtr handle = new IntPtr(mainWindowElement.Current.NativeWindowHandle);
            User32.SetForegroundWindow(handle);
            User32.SetFocus(handle);
            await Simulate.Events().ClickChord(KeyCode.Menu, KeyCode.Q).Wait(200).ClickChord(KeyCode.Return).Invoke().ConfigureAwait(false);
            OnThisForcedExit?.Invoke(this, EventArgs.Empty);
            if (mainWindowElement.IsAlive())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the number of participants from the element(s) and refresh ParticipantCount.
        /// </summary>
        public void RefreshParticipantCount()
        {
            if (!participantsElement.IsAlive())
            {
                FindParticipantsCountElement();
                if (participantsElement.IsAlive())
                {
                    OnParticipantCountAvailable?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    return;
                }
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



        /// <summary>
        /// Find the main window of the zoom.
        /// </summary>
        private void FindWindowElement()
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
                    //TryGetMainWindow is heavy so we have to avoid calling OnEntered event etc more than once.
                    if (mainWindowElement != null)
                    {
                        return;
                    }

                    mainWindowElement = AutomationElement.FromHandle(windowHandle);
                    if (isMinimized)
                    {
                        ZoomState = ZoomState.Minimized;
                    }
                    else
                    {
                        ZoomState = ZoomState.Normal;
                    }
                    OnEntered?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ZoomState = ZoomState.NotRunning;
                }
            }
        }

        /// <summary>
        /// Find the element from which the number of participants can be gotten.
        /// </summary>
        private void FindParticipantsCountElement()
        {
            FindWindowElement();

            if(!GetState())
            {
                ParticipantCount = null;
            }

            bool GetState()
            {
                if (ZoomState == ZoomState.NotRunning || ZoomState == ZoomState.Minimized)
                {
                    //zoom is not ready!
                    return false;
                }

                //1. get participants from the text near the button
                var controlPanelElement = mainWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "ZPControlPanelClass"));
                if (controlPanelElement == null)
                {
                    //not expected
                    ZoomState = ZoomState.NotExpectedBehaviour;
                    //throw new NotImplementedException($"{nameof(controlPanelElement)} not found.");
                    return false;
                }

                var controlPanelChildElement = controlPanelElement.FindFirst(TreeScope.Children, Condition.TrueCondition);
                if (controlPanelChildElement == null)
                {
                    //not expected
                    ZoomState = ZoomState.NotExpectedBehaviour;
                    //throw new NotImplementedException($"{nameof(controlPanelChildElement)} not found.");
                    return false;
                }

                var controlPanelButtonElements = controlPanelElement.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                var buttonElement = controlPanelButtonElements.Cast<AutomationElement>().Where(s => Regex.IsMatch(s.Current.Name, @"\d"));
                if (buttonElement.Any())
                {
                    //window was big enough so that we could find the desired button.
                    ZoomState = ZoomState.Normal;
                    participantsElement = buttonElement.First();
                    return true;
                }
                else
                {
                    //window was too small so the button is not displayed.
                    ZoomState = ZoomState.WindowTooSmall;
                    return false;
                }
            }

            // this makes the program too complex so it is removed.
            /*
            //2. get participants from the text in the participants' list
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
            */
        }
    }
}
