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

        private void GetNumber()
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

        public void OnElapsed(object sender, ElapsedEventArgs e)
        {
            GetNumber();
            OnTimed.Invoke();
        }


        double RandomRange(double min, double max)
        {
            var rand = new Random();
            double randDouble = rand.NextDouble();
            double result = min + (max - min) * randDouble;
            return result;
        }
    }
}
/*MIT License

Copyright (c) 2021 34j and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
