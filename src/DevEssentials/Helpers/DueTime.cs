using System;
using System.Threading.Tasks;

namespace Essentials
{
    public class DueTime
    {
        private DateTime _lastTime = DateTime.MinValue;

        public DueTime(TimeSpan interval)
        {
            Interval = interval;
        }

        public TimeSpan Interval { get; set; }

        public TimeSpan Remaining
        {
            get
            {
                if (_lastTime == DateTime.MinValue)
                    return TimeSpan.Zero;

                DateTime dueTime = _lastTime + Interval;
                TimeSpan remaining = dueTime - DateTime.UtcNow;
                return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
            }
        }

        public bool IsDue => Remaining == TimeSpan.Zero;

        public void Reset() => _lastTime = DateTime.UtcNow;

        public async Task WaitAndReset()
        {
            await Task.Delay(Remaining);
            Reset();
        }

    }
}
