using Prism.Mvvm;
using System;
using System.Timers;

namespace ZoomCloserJp.Models
{
    class Model : BindableBase
    {
        public delegate void TimedEventHandler();
        public event TimedEventHandler OnTimed;
        Timer timer = new Timer();

        public readonly int leastMemberCount = 3;
        double proportion = 0.5f;
        public int ExitNumber { get; private set; } = 0;
        public bool CanExit { get; private set; }

        public ZoomHandler zoomHandler = new ZoomHandler();
        public int MaxNumber { get; private set; } = 0;
        
        void GetNumber()
        {
            var numN = zoomHandler.GetParticipantNumbers();
            if(numN == null)
            {
                return;
            }
            int num = (int)numN;
            if (num > MaxNumber) MaxNumber = num;
            ExitNumber = (int)(MaxNumber * proportion);
            CanExit = (MaxNumber > leastMemberCount);
            if (num < ExitNumber && CanExit)
            {
                zoomHandler.CloseZoom();
            }
        }
        public Model()
        {
            proportion = RandomRange(0.4, 0.7);
            zoomHandler.GetWHs();
            timer.Interval = 100;
            timer.AutoReset = true;
            timer.Elapsed += OnElapsed;
            timer.Enabled = true;

            /* zoomが開いていない時→parentWH.isNull→諦め
             * zoomが最小化されている時→？諦め？
             * zoomが開いているが参加者ウィンドウが開いていない時→zPlistWndClassWH.IsNull(のみ)true→
             * 
             */
        }

        public void OnElapsed(object sender, ElapsedEventArgs e)
        {
            GetNumber();
            OnTimed.Invoke();
        }


        public double RandomRange(double min, double max)
        {
            var rand = new Random();
            double randDouble = rand.NextDouble();
            double result = min + (max - min) * randDouble;
            return result;
        }






    }
}
