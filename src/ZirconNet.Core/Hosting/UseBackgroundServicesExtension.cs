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

            var serviceProvider = services.BuildServiceProvider();
            var hostApplicationLifetimeHandler = serviceProvider.GetRequiredService<HostApplicationLifetimeHandler>();
            var hostedServices = serviceProvider.GetServices<IHostedService>();

            foreach (var hostedService in hostedServices)
            {
                    _ = Task.Run(async () => await hostedService.StartAsync(hostApplicationLifetimeHandler.Token));
            }

            hostApplicationLifetimeHandler.RegisterHostedServicesForShutdown();
        }));

        return builder;
    }
}
