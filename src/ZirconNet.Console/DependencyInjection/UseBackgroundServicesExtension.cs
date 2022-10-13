using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace ZirconNet.Console.DependencyInjection;
public static class UseBackgroundServicesExtension
{
    private static readonly CancellationTokenSource _cts = new();
    private static readonly List<IHostedService> _runningHostedServices = new();

    public static IHostBuilder UseBackgroundServices(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => Task.Run(() =>
        {
            foreach (var service in services)
            {
                if (service is IHostedService hostedService)
                {
                    Task.Run(async () => await hostedService.StartAsync(_cts.Token));
                    _runningHostedServices.Add(hostedService);
                }
            }
        }));

        return builder;
    }
}
