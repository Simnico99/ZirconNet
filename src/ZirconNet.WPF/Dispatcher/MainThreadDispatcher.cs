using System.Threading.Tasks;
using System.Windows;

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

    public void Invoke(Action action)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            action();

            return;
        }

        _dispatcher.Invoke(action);
    }

    public async Task InvokeAsync(Action action)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            action();

            return;
        }

        await _dispatcher.InvokeAsync(action);
    }

    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        if (Environment.CurrentManagedThreadId == _mainThreadId)
        {
            return func();
        }

        return await _dispatcher.InvokeAsync(func);
    }

}

