using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Windows;
using ZirconNet.WPF.Extensions;

namespace ZirconNet.WPF.Hosting.Lifetime;

public static class UseWpfApplicationLifetimeExtension
{
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
            _ = services.AddSingleton<IHostLifetime, WpfApplicationLifetime>();
            _ = services.Configure(configureOptions);
        });
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="window">The <see cref="Window" /> mainwindows to show and wait to close.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this IHostBuilder builder, CancellationToken cancellationToken = default) where T : Window
    {
        builder.UseWpfApplicationLifetime();
        return RunWpfApplicationAsyncInternal<T>(builder, cancellationToken);
    }

    /// <summary>
    /// Enables Wpf support, builds and starts the host, and waits for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit to shut down.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder" /> to configure.</param>
    /// <param name="window">The <see cref="Window" /> mainwindows to show and wait to close.</param>
    /// <param name="configureOptions">The delegate for configuring the <see cref="WpfApplicationLifetimeOptions"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the console.</param>
    /// <returns>A <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.</returns>
    public static Task RunWpfApplicationAsync<T>(this IHostBuilder builder, Action<WpfApplicationLifetimeOptions> configureOptions, CancellationToken cancellationToken = default) where T : Window
    {
        builder.UseWpfApplicationLifetime(configureOptions);
        return RunWpfApplicationAsyncInternal<T>(builder, cancellationToken);
    }

    private async static Task RunWpfApplicationAsyncInternal<T>(IHostBuilder builder, CancellationToken cancellationToken) where T : Window
    {
        builder.ConfigureServices(services => services.AddSingleton<T>());

        using var host = builder.Build();
        var window = host.Services.GetRequiredService<T>();
        var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopped.Register(() => host?.Dispose());

        await host.StartAsync(cancellationToken);
        await window.ShowDialogAsync();
        await host.StopAsync();
    }
}
