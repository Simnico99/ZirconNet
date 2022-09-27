using Microsoft.Extensions.Hosting;

namespace ZirconNet.Console.DependencyInjection;
public static class UseBackgroundServicesExtension
{
    private static readonly CancellationTokenSource _cts = new();
    private static readonly TaskFactory _taskFactory = new(_cts.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, null);
    private static readonly List<IHostedService> _runningHostedServices = new();

    public static IHostBuilder UseBackgroundServices(this IHostBuilder builder)
    {
        builder.ConfigureServices((_, services) =>
            {
                foreach (var service in services)
                {
                    if (service is IHostedService hostedService)
                    {
                        _taskFactory.StartNew(async () => await hostedService.StartAsync(_cts.Token));
                        _runningHostedServices.Add(hostedService);
                    }
                }
            });

        return builder;
    }
}
