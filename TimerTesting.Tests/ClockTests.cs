using NSubstitute;
using TimerTesting.Logic;
using Xunit;

namespace TimerTesting.Tests
{
    public class ClockTests
    {
        [Fact]
        public void Creation()
        {
            var clock = new Clock(Substitute.For<TimeProvider>());

            Assert.NotNull(clock);
        }

        [Theory]
        [InlineData(1, 2, 3, "01:02:03")]
        [InlineData(12, 59, 59, "12:59:59")]
        [InlineData(13, 0, 0, "01:00:00")]
        [InlineData(0, 2, 3, "12:02:03")]
        public void DisplaysCorrectTime (int h, int m, int s, string expected)
        {
            var provider = Substitute.For<TimeProvider>();

            provider.GetUtcNow().Returns(new DateTimeOffset(2023, 12, 25, h, m, s, default));

            var clock = new Clock(provider);

            Assert.Equal(expected, clock.DisplayedTime);
        }

        [Fact]
        public void TickFiresEvent () 
        {
            var provider = Substitute.For<TimeProvider>();

            TimerCallback tickCallback = o => { };

            provider.GetUtcNow().Returns(new DateTimeOffset(2023, 12, 25, 12, 15, 30, default));

            provider.CreateTimer(
                Arg.Do<TimerCallback>(c => tickCallback = c),
                Arg.Any<object>(),
                Arg.Any<TimeSpan>(),
                Arg.Is(new TimeSpan(0, 0, 1))
            );

            var clock = new Clock(provider);

            int fireCount = 0;

            clock.TimeChanged += (s, e) => fireCount++;

            tickCallback(null);

            Assert.Equal(1, fireCount);
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(0, 12)]
        [InlineData(12, 12)]
        [InlineData(15, 3)]
        public void RingCountCorrect(int hour, int count)
        {
            var provider = Substitute.For<TimeProvider>();
            var timer = Substitute.For<ITimer>();

            TimeSpan? period = null;

            timer.Change(Arg.Any<TimeSpan>(), Arg.Do<TimeSpan>(ts => period = ts));

            TimerCallback tickCallback = o => { };
            TimerCallback ringCallback = o => { };

            provider.GetUtcNow().Returns(new DateTimeOffset(2023, 12, 25, hour, 0, 0, default));

            provider.CreateTimer(
                Arg.Do<TimerCallback>(c => tickCallback = c),
                Arg.Any<object>(),
                Arg.Any<TimeSpan>(),
                Arg.Is(new TimeSpan(0, 0, 1))
            );

            provider.CreateTimer(
                Arg.Do<TimerCallback>(c => ringCallback = c),
                Arg.Any<object>(),
                Arg.Any<TimeSpan>(),
                Arg.Is(new TimeSpan(0, 0, 0, 0, 500))
            ).Returns(timer);

            var clock = new Clock(provider);

            int ringOnCount = 0;
            int ringOffCount = 0;

            clock.RingChanged += (s, e) =>
            {
                if (e == RingState.On)
                    ringOnCount++;
                else
                    ringOffCount++;
            };

            tickCallback(null);

            while (period == null || period.Value != Timeout.InfiniteTimeSpan)
                ringCallback(null);

            Assert.Equal(count, ringOnCount);
            Assert.Equal(count, ringOffCount);
        }
    }
}