using System;
using System.Timers;

namespace TimerTesting.Logic
{
    public enum RingState { Off, On }

    public class Clock 
    {
        public string DisplayedTime => DateTime.Now.ToString("HH:mm:ss");

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

                if (e.SignalTime.Minute == 0 && e.SignalTime.Second == 0)
                    FireRinging(e);
            };
            
            _timer.Start();
        }

        private void FireRinging(ElapsedEventArgs e)
        {
            int countdown = e.SignalTime.Hour;

            RingState state = RingState.Off;

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
