using TimerTesting.Logic;

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