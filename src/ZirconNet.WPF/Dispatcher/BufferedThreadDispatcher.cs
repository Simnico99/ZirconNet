using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

/// <summary>
/// Prevent the ui freeze by using a buffer to execute the UI Updates from multiple threads in a sort of queue.
/// </summary>
public sealed class BufferedThreadDispatcher
{
    private static readonly BufferedThreadDispatcher _current = new();

    public static BufferedThreadDispatcher Current => _current;
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(1);

    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;
    private readonly Queue<Action> _queue = new(25);
    private readonly object _queueLock = new();
    private readonly Thread _uiThread;
    private readonly TaskCompletionSource<object?> _taskCompletionSource = new();

    private BufferedThreadDispatcher()
    {
        _mainThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;

        _uiThread = new Thread(ProcessQueue);
        _uiThread.SetApartmentState(ApartmentState.STA);
        _uiThread.IsBackground = true;
        _uiThread.Start();
    }

    private async void ProcessQueue()
    {
        var timer = new Timer(_ => ExecuteQueuedActions(), null, 0, (int)Delay.TotalMilliseconds);

        await _taskCompletionSource.Task;
    }

    private void ExecuteQueuedActions()
    {
        Action? action = null;

        lock (_queueLock)
        {
            if (_queue.Count > 0)
            {
                action = _queue.Dequeue();
            }
        }

        while (action != null)
        {
            if (_mainThreadId == Environment.CurrentManagedThreadId)
            {
                action();
            }
            else
            {
                _dispatcher.Invoke(action, DispatcherPriority.Send);
            }

            action = null;

            lock (_queueLock)
            {
                if (_queue.Count > 0)
                {
                    action = _queue.Dequeue();
                }
            }
        }
    }

    public void Invoke(Action action)
    {
        lock (_queueLock)
        {
            _queue.Enqueue(action);
        }
    }

    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();

        lock (_queueLock)
        {
            _queue.Enqueue(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
        }

        return await tcs.Task.ConfigureAwait(false);
    }

    public async Task InvokeAsync(Action act)
    {
        var tcs = new TaskCompletionSource<object>();

        lock (_queueLock)
        {
            _queue.Enqueue(() =>
            {
                try
                {
                    act();
                    tcs.SetResult(default!);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
        }

        await tcs.Task.ConfigureAwait(false);
    }
}