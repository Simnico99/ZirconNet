using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZirconNet.Core.Hosting;

/// <summary>
/// Handles the IHostApplicationLifetime so it stop background services on stop etc...
/// </summary>
public class HostApplicationLifetimeHandler
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceProvider _services;
    public CancellationToken Token { get; }

    public HostApplicationLifetimeHandler(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider services)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _services = services;

        Token = _hostApplicationLifetime.ApplicationStopping;
    }

    internal void RegisterHostedServicesForShutdown()
    {
        foreach (var hostedService in _services.GetServices<IHostedService>())
        {
            _ = _hostApplicationLifetime.ApplicationStopping.Register(async () => await hostedService.StopAsync(_hostApplicationLifetime.ApplicationStopped));
        }
    }
}
