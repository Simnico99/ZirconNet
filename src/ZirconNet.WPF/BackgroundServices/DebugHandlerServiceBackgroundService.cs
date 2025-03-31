// <copyright file="DebugHandlerServiceBackgroundService.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Hosting;
using ZirconNet.Core.Environments;
using ZirconNet.WPF.Interops;

namespace ZirconNet.WPF.BackgroundServices;

internal sealed class DebugHandlerBackgroundService : BackgroundService
{
    private readonly IEnvironmentManager _environmentManager;

    public DebugHandlerBackgroundService(IEnvironmentManager environmentManager)
    {
        _environmentManager = environmentManager;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_environmentManager.IsDebug)
        {
            InteropMethods.CreateConsole();
        }

        return Task.CompletedTask;
    }
}
