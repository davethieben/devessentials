using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Essentials.Helpers
{
    /// <summary>
    /// simple helper for Semaphores to allow a "using" block to ensure proper release,
    /// even if exceptions are thrown.
    /// </summary>
    public class AsyncLock
    {
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _semaphores = new ConcurrentDictionary<object, SemaphoreSlim>();

        private SemaphoreSlim Get(object? key = null) => _semaphores.GetOrAdd(key ?? this, _ => new SemaphoreSlim(1));

        public async Task<IDisposable> AcquireAsync(object? key = null)
        {
            key = key ?? this;

            var semaphore = Get(key);
            await semaphore.WaitAsync();

            return new Disposable(() =>
            {
                semaphore.Release();
                _semaphores.TryRemove(key, out semaphore);
            });
        }

        public IDisposable Acquire(object? key = null)
        {
            key = key ?? this;

            var semaphore = Get(key);
            semaphore.Wait();

            return new Disposable(() =>
            {
                semaphore.Release();
                _semaphores.TryRemove(key, out semaphore);
            });
        }

        public bool TryGetLock(out IDisposable? locker)
        {
            if (!IsLocked())
            {
                lock (this)
                {
                    if (!IsLocked())
                    {
                        locker = Acquire();
                        return true;
                    }
                }
            }

            locker = null;
            return false;
        }

        public bool IsLocked(object? key = null) => Get(key ?? this).CurrentCount == 0;

    }
}
