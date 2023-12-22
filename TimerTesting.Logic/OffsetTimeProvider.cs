using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerTesting.Logic
{
    public class OffsetTimeProvider : TimeProvider
    {
        private readonly TimeSpan _offsetTime;

        public OffsetTimeProvider()
        {
            var now = base.GetUtcNow();

            _offsetTime = now - new DateTimeOffset (now.Year, now.Month, now.Day, now.Hour, 59, 55, default);
        }

        public override DateTimeOffset GetUtcNow()
        {
            return base.GetUtcNow() - _offsetTime;
        }
    }
}
