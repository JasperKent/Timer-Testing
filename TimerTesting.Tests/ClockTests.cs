using TimerTesting.Logic;
using Xunit;

namespace TimerTesting.Tests
{
    public class ClockTests
    {
        [Fact]
        public void Creation()
        {
            var clock = new Clock();

            Assert.NotNull(clock);
        }
    }
}