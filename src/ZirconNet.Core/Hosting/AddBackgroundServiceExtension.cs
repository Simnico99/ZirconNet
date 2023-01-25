using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Hosting;
public static class AddBackgroundServiceExtension
{
    /// <summary>
    /// Register the Services with itself, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddBackgroundServices<TImplementation>(this IServiceCollection services)
        where TImplementation : BackgroundService
    {
        _ = services.AddSingleton<TImplementation>();
        _ = services.AddSingleton<BackgroundService, TImplementation>(x => x.GetRequiredService<TImplementation>());
        _ = services.AddHostedService(x => x.GetRequiredService<TImplementation>());

        return services;
    }

    /// <summary>
    /// Register the Services with itself, the provided interface, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <param name="services">The provided <see cref="IServiceCollection"/>.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddBackgroundServices<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : BackgroundService, TService
    {
        _ = services.AddSingleton<TImplementation>();
        _ = services.AddSingleton<TService, TImplementation>(x => x.GetRequiredService<TImplementation>());
        _ = services.AddSingleton<BackgroundService, TImplementation>(x => x.GetRequiredService<TImplementation>());
        _ = services.AddHostedService(x => x.GetRequiredService<TImplementation>());

        return services;
    }
}
