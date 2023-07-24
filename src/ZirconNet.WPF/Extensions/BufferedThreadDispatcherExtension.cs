// <copyright file="BufferedThreadDispatcherExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Hosting;
using ZirconNet.Core.Hosting;
using ZirconNet.WPF.BackgroundServices;

namespace ZirconNet.WPF.Extensions;

public static class BufferedThreadDispatcherExtension
{
    public static IHostBuilder UseBufferedThreadDispatcher(this IHostBuilder builder)
    {
        builder.ConfigureServices(services => services.AddBackgroundServices<BufferedDispatcherBackgroundService>());
        return builder;
    }
}
