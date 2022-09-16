using System.Windows;

namespace ZirconNet.WPF.Dispatcher;
public sealed class ThreadDispatcher
{
    public static ThreadDispatcher Current { get; } = new();

    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;

    private ThreadDispatcher()
    {
        _mainThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;
    }

    public void Invoke(Action action)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        _dispatcher.Invoke(action);
    }

    public async ValueTask InvokeAsync(Action action)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        await _dispatcher.InvokeAsync(action);
    }

    public async ValueTask<T> InvokeAsync<T>(Func<T> func)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            return func();
        }

        return await _dispatcher.InvokeAsync(func);
    }
}