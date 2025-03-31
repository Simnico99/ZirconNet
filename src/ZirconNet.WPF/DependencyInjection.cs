// <copyright file="DependencyInjection.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using ZirconNet.Microsoft.DependencyInjection.Hosting;
using ZirconNet.WPF.BackgroundServices;

namespace ZirconNet.WPF;

public static class DependencyInjection
{
    public static IServiceCollection AddDebugConsole(this IServiceCollection services)
    {
        services.AddBackgroundServices<DebugHandlerBackgroundService>();
        return services;
    }
}
