using ZirconNet.Core.Events;

namespace ZirconNet.WPF.Mvvm;
public abstract class AwaitableViewModel : ViewModel
{
    public IWeakEvent<AwaitableViewModel> ReadyEvent { get; } = new WeakEvent<AwaitableViewModel>();
    private TaskCompletionSource<object>? _startupTcs;

    public AwaitableViewModel()
    {
        RegisterViewModelReady();
    }

    public void IsReady()
    {
        ReadyEvent.Publish(this);
    }

    public Task WaitForReadyAsync(CancellationToken cancellationToken = default)
    {
        if (_startupTcs!.Task.IsCompleted)
        {
            return _startupTcs!.Task;
        }

        var registration = cancellationToken.Register(delegate (object? state)
        {
            if (state != null)
            {
                _ = ((TaskCompletionSource<object>)state).TrySetResult(new object());
            }
        }, _startupTcs);
        return _startupTcs!.Task.ContinueWith((Task<object> _) => registration.Dispose(), cancellationToken);
    }

    internal void RegisterViewModelReady()
    {
        _startupTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        using (ReadyEvent.Subscribe((_) => _startupTcs!.TrySetResult(new object()))) { }
    }
}
