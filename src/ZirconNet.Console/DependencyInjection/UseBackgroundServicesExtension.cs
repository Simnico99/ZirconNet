using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

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
                    _taskFactory.StartNew(() => hostedService.StartAsync(_cts.Token), TaskCreationOptions.RunContinuationsAsynchronously | TaskCreationOptions.LongRunning);
                    _runningHostedServices.Add(hostedService);
                }
            }
        });

        return builder;
    }
}
