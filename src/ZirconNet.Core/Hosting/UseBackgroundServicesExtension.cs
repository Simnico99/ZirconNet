using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZirconNet.Core.Hosting;

/// <summary>
/// Start all background services and register them for IHostApplicationLifetime stop.
/// </summary>
public static class UseBackgroundServicesExtension
{
    public static IHostBuilder UseBackgroundServices(this IHostBuilder builder)
    {
        _ = builder.ConfigureServices((context, services) => Task.Run(() =>
        {
            _ = services.AddSingleton<HostApplicationLifetimeHandler>();

            foreach (var service in services)
            {
                if (service is IHostedService hostedService)
                {
                    _ = Task.Run(async () => await hostedService.StartAsync(HostApplicationLifetimeHandler.Token));
                }
            }
        }));

        return builder;
    }
}
