using System;
using System.Collections.Generic;

namespace Essentials.Helpers
{
    public class Disposable : IDisposable
    {
        public static readonly Disposable NullDisposable = new Disposable();

        private readonly Action? _onDispose;

        public Disposable(Action? onDispose = null) => _onDispose = onDispose;

        public Disposable(IEnumerable<IDisposable> disposables)
        {
            if (!disposables.IsNullOrEmpty())
            {
                _onDispose = () =>
                {
                    foreach (var disposable in disposables)
                    {
                        disposable?.Dispose();
                    }
                };
            }
        }

        public void Dispose() => _onDispose?.Invoke();

    }
}
