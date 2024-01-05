using Microsoft.Extensions.Time.Testing;
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
            var clock = new Clock(new FakeTimeProvider());

            Assert.NotNull(clock);
        }

        [Theory]
        [InlineData(1, 2, 3, "01:02:03")]
        [InlineData(12, 59, 59, "12:59:59")]
        [InlineData(13, 0, 0, "01:00:00")]
        [InlineData(0, 2, 3, "12:02:03")]
        public void DisplaysCorrectTime (int h, int m, int s, string expected)
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2023, 12, 25, h, m, s, default));

            var clock = new Clock(provider);

            Assert.Equal(expected, clock.DisplayedTime);
        }

        [Fact]
        public void TickFiresEvent () 
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2023, 12, 25, 1, 2, 3, default));

            var clock = new Clock(provider);

            int fireCount = 0;

            clock.TimeChanged += (s, e) => fireCount++;

            provider.Advance(new TimeSpan(0, 0, 1));

            Assert.Equal(1, fireCount);
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(0, 12)]
        [InlineData(12, 12)]
        [InlineData(15, 3)]
        public void RingCountCorrect(int hour, int count)
        {
            var provider = new FakeTimeProvider(new DateTimeOffset(2023, 12, 25, hour, 0, 0, default) - new TimeSpan(0,0,1));
                        
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

            for (int i = 0; i < 13; i++) 
                provider.Advance(new TimeSpan (0,0,1));

            Assert.Equal(count, ringOnCount);
            Assert.Equal(count, ringOffCount);
        }
    }
}