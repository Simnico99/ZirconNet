using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;
public sealed class MainThreadDispatcher : IMainThreadDispatcher
{
    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;

    public MainThreadDispatcher()
    {
        _mainThreadId = Environment.CurrentManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;
    }

    public void Invoke(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            action();

            return;
        }

        _dispatcher.Invoke(action, dispatcherPriority);
    }

    public async ValueTask InvokeAsync(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            action();

            return;
        }

        await _dispatcher.InvokeAsync(action, dispatcherPriority);
    }

    public async ValueTask<T> InvokeAsync<T>(Func<T> func, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        return Environment.CurrentManagedThreadId == _mainThreadId ? func() : await _dispatcher.InvokeAsync(func, dispatcherPriority);
    }
}