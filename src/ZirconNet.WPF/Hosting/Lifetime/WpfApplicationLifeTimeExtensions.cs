﻿// <copyright file="WpfApplicationLifeTimeExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Windows;
using ZirconNet.Core.Environments;
using ZirconNet.WPF.Debugging;
using ZirconNet.WPF.Extensions;

namespace ZirconNet.WPF.Hosting.Lifetime;

public static class WpfApplicationLifeTimeExtensions
{
#if NET6_0_OR_GREATER

    /// <summary>
    /// Listens for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit and calls <see cref="IHostApplicationLifetime.StopApplication"/> to start the shutdown process.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public static IHostApplicationBuilder UseWpfApplicationLifetime(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IHostLifetime, WpfApplicationLifetime>();
        return builder;
    }

    /// <summary>
    /// Listens for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit and calls <see cref="IHostApplicationLifetime.StopApplication"/> to start the shutdown process.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="configureOptions">The delegate for configuring the <see cref="WpfApplicationLifetimeOptions"/>.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public static IHostApplicationBuilder UseWpfApplicationLifetime(this IHostApplicationBuilder builder, Action<WpfApplicationLifetimeOptions> configureOptions)
    {
        builder.Services.AddSingleton<IHostLifetime, WpfApplicationLifetime>();
        builder.Services.Configure(configureOptions);
        return builder;
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <typeparam name="T">The <see cref="Window" /> mainwindows to show and wait to close.</typeparam>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this HostApplicationBuilder builder, bool showConsoleWithDebugger = false, bool showConsoleWithDebugEnvironment = false, CancellationToken cancellationToken = default)
        where T : Window
    {
        builder.UseWpfApplicationLifetime();
        return RunWpfApplicationAsyncInternal<T>(builder, showConsoleWithDebugger, showConsoleWithDebugEnvironment, cancellationToken);
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <typeparam name="T">The <see cref="Window" /> mainwindows to show and wait to close.</typeparam>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="configureOptions">The delegate for configuring the <see cref="WpfApplicationLifetimeOptions"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this HostApplicationBuilder builder, Action<WpfApplicationLifetimeOptions> configureOptions, bool showConsoleWithDebugger = false, bool showConsoleWithDebugEnvironment = false, CancellationToken cancellationToken = default)
        where T : Window
    {
        builder.UseWpfApplicationLifetime(configureOptions);
        return RunWpfApplicationAsyncInternal<T>(builder, showConsoleWithDebugger, showConsoleWithDebugEnvironment, cancellationToken);
    }

    private static async Task RunWpfApplicationAsyncInternal<T>(HostApplicationBuilder builder, bool showConsoleWithDebugger, bool showConsoleWithDebugEnvironment, CancellationToken cancellationToken)
        where T : Window
    {
        ToggleDebugConsole(showConsoleWithDebugger, showConsoleWithDebugEnvironment);
        builder.Services.AddSingleton<T>();
        using var host = builder.Build();
        var window = host.Services.GetRequiredService<T>();
        var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopped.Register(() => host?.Dispose());

        await host.StartAsync(cancellationToken).ConfigureAwait(false);
        await window.ShowDialogAsync().ConfigureAwait(false);
        ToggleDebugConsole(showConsoleWithDebugger, showConsoleWithDebugEnvironment);
        await host.StopAsync(cancellationToken).ConfigureAwait(false);
    }

#else

    /// <summary>
    /// Listens for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit and calls <see cref="IHostApplicationLifetime.StopApplication"/> to start the shutdown process.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public static IHostBuilder UseWpfApplicationLifetime(this IHostBuilder builder)
    {
        return builder.ConfigureServices((services) => services.AddSingleton<IHostLifetime, WpfApplicationLifetime>());
    }

    /// <summary>
    /// Listens for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit and calls <see cref="IHostApplicationLifetime.StopApplication"/> to start the shutdown process.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="configureOptions">The delegate for configuring the <see cref="WpfApplicationLifetimeOptions"/>.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public static IHostBuilder UseWpfApplicationLifetime(this IHostBuilder builder, Action<WpfApplicationLifetimeOptions> configureOptions)
    {
        return builder.ConfigureServices((services) =>
        {
            services.TryAddSingleton<IHostLifetime, WpfApplicationLifetime>();
            services.Configure(configureOptions);
        });
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <typeparam name="T">The <see cref="Window" /> mainwindows to show and wait to close.</typeparam>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this IHostBuilder builder, bool showConsoleWithDebugger = false, bool showConsoleWithDebugEnvironment = false, CancellationToken cancellationToken = default)
        where T : Window
    {
        builder.UseWpfApplicationLifetime();
        return RunWpfApplicationAsyncInternal<T>(builder, showConsoleWithDebugger, showConsoleWithDebugEnvironment, cancellationToken);
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <typeparam name="T">The <see cref="Window" /> mainwindows to show and wait to close.</typeparam>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="configureOptions">The delegate for configuring the <see cref="WpfApplicationLifetimeOptions"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this IHostBuilder builder, Action<WpfApplicationLifetimeOptions> configureOptions, bool showConsoleWithDebugger = false, bool showConsoleWithDebugEnvironment = false, CancellationToken cancellationToken = default)
        where T : Window
    {
        builder.UseWpfApplicationLifetime(configureOptions);
        return RunWpfApplicationAsyncInternal<T>(builder, showConsoleWithDebugger, showConsoleWithDebugEnvironment, cancellationToken);
    }

    private static async Task RunWpfApplicationAsyncInternal<T>(IHostBuilder builder, bool showConsoleWithDebugger, bool showConsoleWithDebugEnvironment, CancellationToken cancellationToken)
        where T : Window
    {
        ToggleDebugConsole(showConsoleWithDebugger, showConsoleWithDebugEnvironment);
        builder.ConfigureServices(services => services.AddSingleton<T>());
        using var host = builder.Build();
        var window = host.Services.GetRequiredService<T>();
        var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopped.Register(() => host?.Dispose());

        await host.StartAsync(cancellationToken).ConfigureAwait(false);
        await window.ShowDialogAsync().ConfigureAwait(false);
        ToggleDebugConsole(showConsoleWithDebugger, showConsoleWithDebugEnvironment);
        await host.StopAsync(cancellationToken).ConfigureAwait(false);
    }

#endif

    private static void ToggleDebugConsole(bool showConsoleWithDebugger, bool showConsoleWithDebugEnvironment)
    {
        if ((EnvironmentManager.Current.IsDebug && showConsoleWithDebugEnvironment)
            || (Debugger.IsAttached && showConsoleWithDebugger))
        {
            ConsoleManager.Toggle();
        }
    }
}
