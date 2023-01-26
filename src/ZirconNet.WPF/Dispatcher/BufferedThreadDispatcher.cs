using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

/// <summary>
/// Prevent the ui freeze by using a buffer to execute the UI Updates from multiple threads in a sort of queue.,
/// </summary>
public sealed class BufferedThreadDispatcher
{
    public static BufferedThreadDispatcher Current { get; } = new();

    /// <summary>
    /// Delay to wait between the screen refresh.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(1);

    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;
    private readonly ConcurrentQueue<Action> _queue = new();

    private BufferedThreadDispatcher()
    {
        _mainThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;
        Task.Run(ProcessQueue);
    }

    private async void ProcessQueue()
    {
        while (true)
        {
            if (_queue.TryDequeue(out var action))
            {
                if (_mainThreadId == Environment.CurrentManagedThreadId)
                {
                    action();
                }
                else
                {
                    _dispatcher.Invoke(action, DispatcherPriority.Send);
                }
            }
            await Task.Delay(Delay);
        }
    }

    public void Invoke(Action action)
    {
        _queue.Enqueue(action);
    }

    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();
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
        return await tcs.Task;
    }

    public async Task InvokeAsync(Action act)
    {
        var tcs = new TaskCompletionSource<object?>();
        _queue.Enqueue(() =>
        {
            try
            {
                act();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        await tcs.Task;
    }
}