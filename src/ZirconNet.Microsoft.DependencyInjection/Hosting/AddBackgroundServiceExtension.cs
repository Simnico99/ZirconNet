// <copyright file="AddBackgroundServiceExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ZirconNet.Microsoft.DependencyInjection.Hosting;

public static class AddBackgroundServiceExtension
{
    /// <summary>
    /// Register the Services with itself, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="TImplementation">The type to add.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddBackgroundServices<TImplementation>(this IServiceCollection services)
        where TImplementation : BackgroundService
    {
        services.TryAddSingleton<TImplementation>();
        services.TryAddSingleton<BackgroundService>(provider => provider.GetRequiredService<TImplementation>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, TImplementation>(provider => provider.GetRequiredService<TImplementation>()));

        return services;
    }

    /// <summary>
    /// Register the Services with itself, the provided interface, <see cref="IHostedService"/> and <see cref="BackgroundService"/>.
    /// </summary>
    /// <typeparam name="TService">The interface to add.</typeparam>
    /// <typeparam name="TImplementation">The type to add.</typeparam>
    /// <param name="services">The provided <see cref="IServiceCollection"/>.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddBackgroundServices<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : BackgroundService, TService
    {
        services.TryAddSingleton<TImplementation>();
        services.TryAddSingleton<TService>(provider => provider.GetRequiredService<TImplementation>());
        services.TryAddSingleton<BackgroundService>(provider => provider.GetRequiredService<TImplementation>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, TImplementation>(provider => provider.GetRequiredService<TImplementation>()));

        return services;
    }
}
