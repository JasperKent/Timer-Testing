using System;
using System.Threading;
using System.Timers;

namespace TimerTesting.Logic
{
    public enum RingState { Off, On }

    public class Clock 
    {
        public string DisplayedTime => _timeProvider.GetUtcNow().LocalDateTime.ToString("hh:mm:ss");

        public event EventHandler<EventArgs>? TimeChanged;
        public event EventHandler<RingState>? RingChanged;

        private ITimer? _ringTimer;

        private readonly TimeProvider _timeProvider;

        public Clock(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;

            _timeProvider.CreateTimer(tickCallback, null, default, new TimeSpan(0, 0, 1));

            void tickCallback(object? o)
            {
                TimeChanged?.Invoke(this, EventArgs.Empty);

                var now = _timeProvider.GetUtcNow().LocalDateTime;

                if (now.Minute == 0 && now.Second == 0)
                    FireRinging(now.Hour);
            };          
        }

        private void FireRinging(int hour)
        {
            int countdown = hour % 12 == 0 ? 12 : hour % 12;

            RingState state = RingState.On;

            _ringTimer ??= _timeProvider.CreateTimer(ringCallback, null, default, new TimeSpan(0, 0, 0, 0, 500));
            _ringTimer.Change(default, new TimeSpan(0, 0, 0, 0, 500));

            void ringCallback(object? o) 
            {
                if (countdown > 0)
                {
                    RingChanged?.Invoke(this, state);

                    if (state == RingState.Off)
                    {
                        --countdown;
                        state = RingState.On;
                    }
                    else
                        state = RingState.Off;

                    if (countdown == 0)
                        _ringTimer?.Change(default, Timeout.InfiniteTimeSpan);
                }
            };
        }
    }
}
