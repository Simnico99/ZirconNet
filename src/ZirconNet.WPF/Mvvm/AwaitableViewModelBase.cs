using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Threading;
using ZirconNet.Core.Events;

namespace ZirconNet.WPF.Mvvm;
public class AwaitableViewModelBase : ObservableObject
{
    public IWeakEvent<AwaitableViewModelBase> ReadyEvent { get; } = new WeakEvent<AwaitableViewModelBase>();
    private TaskCompletionSource<object>? _startupTcs;

    public AwaitableViewModelBase()
    {
        RegisterViewModelReady();
    }

    public void IsReady() => ReadyEvent.PublishAsync(this);

    public void NotifyPropertyChanged(string propertyName) => OnPropertyChanged(propertyName);

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
                ((TaskCompletionSource<object>)state).TrySetResult(new object());
            }
        }, _startupTcs);
        return _startupTcs!.Task.ContinueWith((Task<object> _) => registration.Dispose(), cancellationToken);
    }

    internal void RegisterViewModelReady()
    {
        _startupTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        ReadyEvent.Subscribe((_) =>
        {
            _startupTcs!.TrySetResult(new object());
        });
    }
}
