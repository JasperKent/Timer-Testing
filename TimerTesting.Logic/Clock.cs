using System;
using System.Timers;

namespace TimerTesting.Logic
{
    public enum RingState { Off, On }

    public class Clock 
    {
        public string DisplayedTime => DateTime.Now.ToString("hh:mm:ss");

        public event EventHandler<EventArgs>? TimeChanged;
        public event EventHandler<RingState>? RingChanged;

        private Timer _timer;
        private Timer _ringTimer;

        public Clock()
        {
            _timer = new Timer(new TimeSpan(0, 0, 1));
            _ringTimer = new Timer(new TimeSpan(0, 0, 0, 0, 500));

            _timer.Elapsed += (s, e) =>
            {
                TimeChanged?.Invoke(this, EventArgs.Empty);

                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    FireRinging(DateTime.Now.Hour);
            };
            
            _timer.Start();
        }

        private void FireRinging(int hour)
        {
            int countdown = hour % 12 == 0 ? 12 : hour % 12;

            RingState state = RingState.On;

            _ringTimer.Elapsed += (s, e) =>
            {
                RingChanged?.Invoke(this, state = state == RingState.On ? RingState.Off : RingState.On);

                --countdown;

                if (countdown == 0)
                    _ringTimer.Stop();
            };

            _ringTimer.Start();
        }
    }
}
