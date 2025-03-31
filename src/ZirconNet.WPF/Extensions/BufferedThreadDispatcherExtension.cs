// <copyright file="BufferedThreadDispatcherExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Hosting;
using ZirconNet.Microsoft.DependencyInjection.Hosting;
using ZirconNet.WPF.BackgroundServices;
using ZirconNet.WPF.Dispatcher;

namespace ZirconNet.WPF.Extensions;

public static class BufferedThreadDispatcherExtension
{
#if NET6_0_OR_GREATER
    public static HostApplicationBuilder UseBufferedThreadDispatcher(this HostApplicationBuilder builder, TimeSpan delay = default!)
    {
        if (delay != default!)
        {
            BufferedThreadDispatcher.Current.Delay = delay;
        }

        builder.Services.AddBackgroundServices<BufferedDispatcherBackgroundService>();
        return builder;
    }
#else
    public static IHostBuilder UseBufferedThreadDispatcher(this IHostBuilder builder, TimeSpan delay = default!)
    {
        if (delay != default!)
        {
            BufferedThreadDispatcher.Current.Delay = delay;
        }

        builder.ConfigureServices(services => services.AddBackgroundServices<BufferedDispatcherBackgroundService>());
        return builder;
    }

#endif
}
