﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.WPF.DependencyInjection;
public static class AddBackgroundServiceExtension
{
    /// <summary>
    /// Register the Services with itself, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBackgroundServices<T>(this IServiceCollection services) 
        where T : BackgroundService
    {
        services.AddSingleton<T>();
        services.AddSingleton<BackgroundService, T>(x => x.GetRequiredService<T>());
        services.AddSingleton<IHostedService, T>(x => x.GetRequiredService<T>());

        return services;
    }

    /// <summary>
    /// Register the Services with itself, the provided interface, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddBackgroundServices<TService, TImplementation>(this IServiceCollection services) 
        where TService : class 
        where TImplementation : BackgroundService, TService
    {
        services.AddSingleton<TService>();
        services.AddSingleton<TService, TImplementation>(x => x.GetRequiredService<TImplementation>());
        services.AddSingleton<BackgroundService, TImplementation>(x => x.GetRequiredService<TImplementation>());
        services.AddSingleton<IHostedService, TImplementation>(x => x.GetRequiredService<TImplementation>());

        return services;
    }
}
