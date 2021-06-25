/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Timers;
using ZoomCloser.Modules;

namespace ZoomCloser.Models
{
    class MainWindowModel : BindableBase
    {
        public delegate void TimedEventHandler();
        public event TimedEventHandler OnTimed;
        public event EventHandler OnExit
        {
            add { zoomHandler.OnExit += value; }
            remove { zoomHandler.OnExit -= value; }
        }
        public event EventHandler OnEntered
        {
            add { zoomHandler.OnEntered += value; }
            remove { zoomHandler.OnEntered -= value; }
        }
        public event EventHandler OnForcedExit
        {
            add { zoomHandler.OnThisForcedExit += value; }
            remove { zoomHandler.OnThisForcedExit -= value; }
        }
        public event EventHandler OnParticipantsCountAvailbale
        {
            add { zoomHandler.OnParticipantCountAvailable += value; }
            remove { zoomHandler.OnParticipantCountAvailable -= value; }
        }

        readonly Timer timer = new Timer();

        public int participantCountToThresholdValue { get; private set; } = 3;
        public double proportion { get; private set; } = 0.5f;
        public int ExitNumber { get; private set; } = 0;
        public bool IsOverThreshold { get; private set; }

        private ZoomHandler zoomHandler = new ZoomHandler();
        public int MaxNumber { get; private set; } = 0;
       
        public MainWindowModel()
        {
            proportion = RandomRange(0.4, 0.7);
            timer.Interval = 100;
            timer.AutoReset = true;
            timer.Elapsed += OnElapsed;
            timer.Enabled = true;
        }

        private async Task CheckAndClose()
        {
            var result = await zoomHandler.GetParticipantCount();
            if(result == false)
            {
                return;
            }
            int num = zoomHandler.ParticipantCount;

            MaxNumber = Math.Max(MaxNumber, num);

            ExitNumber = (int)(MaxNumber * proportion);
            IsOverThreshold = MaxNumber > participantCountToThresholdValue;

            if (num < ExitNumber && IsOverThreshold)
            {
               await zoomHandler.CloseZoom();
            }
        }

        public async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            await CheckAndClose();
            OnTimed.Invoke();
        }

        public async Task CloseZoom() => await zoomHandler.CloseZoom();
        public ZoomHandler.ZoomMode ZoomMode => zoomHandler.zoomMode;
        public int ParticipantCount => zoomHandler.ParticipantCount;

        static double RandomRange(double min, double max)
        {
            var rand = new Random();
            double randDouble = rand.NextDouble();
            double result = min + (max - min) * randDouble;
            return result;
        }
    }
}

