using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ZirconNet.WPF.Dispatcher;

namespace ZirconNet.WPF.DependencyInjection;
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

        Application.Current.Exit += CurrentExit;
        return builder;
    }

    private static void CurrentExit(object _, ExitEventArgs __)
    {
        Application.Current.Exit -= CurrentExit;
        _cts.Cancel();
#if NET5_0_OR_GREATER
        foreach (var hostedService in CollectionsMarshal.AsSpan(_runningHostedServices))
        {
            hostedService.StopAsync(default);
        }
#else
        foreach (var hostedService in _runningHostedServices)
        {
            hostedService.StopAsync(default);
        }
#endif
    }
}
