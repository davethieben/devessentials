using System;

namespace Essentials.Helpers
{
    public class Disposable : IDisposable
    {
        public static readonly Disposable NullDisposable = new Disposable();

        private readonly Action? _onDispose;

        public Disposable(Action? onDispose = null) => _onDispose = onDispose;

        public void Dispose() => _onDispose?.Invoke();

    }
}
