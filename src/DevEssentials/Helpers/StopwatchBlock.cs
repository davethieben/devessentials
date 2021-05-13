using System;
using System.Diagnostics;

namespace Essentials.Helpers
{
    public class StopwatchBlock : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Action<TimeSpan> _onComplete;

        public StopwatchBlock(Action<TimeSpan> onComplete)
        {
            _onComplete = onComplete.IsRequired();
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _onComplete.Invoke(_stopwatch.Elapsed);
        }
    }
}
