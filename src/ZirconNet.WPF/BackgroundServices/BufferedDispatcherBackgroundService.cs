// <copyright file="BufferedDispatcherBackgroundService.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Hosting;
using ZirconNet.WPF.Dispatcher;

namespace ZirconNet.WPF.BackgroundServices;

/// <summary>
/// Run the <see cref="BufferedThreadDispatcher"/> in a background thread.
/// </summary>
internal sealed class BufferedDispatcherBackgroundService : BackgroundService
{
    private readonly BufferedThreadDispatcher _currentBufferedDispatcher = BufferedThreadDispatcher.Current;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _currentBufferedDispatcher.ProcessOneItem();
            await Task.Delay(_currentBufferedDispatcher.Delay, stoppingToken);
        }
    }
}
