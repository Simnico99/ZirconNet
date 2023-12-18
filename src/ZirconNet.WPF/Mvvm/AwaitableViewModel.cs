// <copyright file="AwaitableViewModel.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Events;

namespace ZirconNet.WPF.Mvvm;

public abstract class AwaitableViewModel : ViewModel
{
    private TaskCompletionSource<object>? _startupTcs;

    public AwaitableViewModel()
    {
        RegisterViewModelReady();
    }

    public IWeakEvent<AwaitableViewModel> ReadyEvent { get; } = new WeakEvent<AwaitableViewModel>();

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

        var registration = cancellationToken.Register(
            delegate(object? state)
            {
                if (state != null)
                {
                    _ = ((TaskCompletionSource<object>)state).TrySetResult(new object());
                }
            },
            _startupTcs);
        return _startupTcs!.Task.ContinueWith((Task<object> _) => registration.Dispose(), cancellationToken);
    }

    /// <summary>
    /// Registers the view model ready.
    /// </summary>
    internal void RegisterViewModelReady()
    {
        _startupTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        ReadyEvent.Subscribe((_) => _startupTcs!.TrySetResult(new object()));
    }
}
