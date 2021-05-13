namespace System
{
    public delegate void Disposing<T>(T arg);

    public interface INotifyWhenDisposed<T> : IDisposable
    {
        event Disposing<T> OnDisposed;
    }
}
