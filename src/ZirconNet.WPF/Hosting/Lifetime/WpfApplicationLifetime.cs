// <copyright file="WpfApplicationLifetime.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ZirconNet.WPF.Hosting.Lifetime;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
/// <summary>
/// Listens for Application.Current.Exit or AppDomain.CurrentDomain.ProcessExit.
/// </summary>
public sealed class WpfApplicationLifetime : IHostLifetime, IDisposable
{
    private readonly ManualResetEvent _shutdownBlock = new(false);

    private CancellationTokenRegistration? _applicationStartedRegistration;
    private CancellationTokenRegistration? _applicationStoppingRegistration;

    public WpfApplicationLifetimeOptions Options { get; }

    private IHostApplicationLifetime ApplicationLifetime { get; }

    public IHostEnvironment Environment { get; }

    public HostOptions HostOptions { get; }

    private ILogger Logger { get; }

    public WpfApplicationLifetime(IOptions<WpfApplicationLifetimeOptions> options, IHostApplicationLifetime hostApplicationLifetime, IHostEnvironment environment, IOptions<HostOptions> hostOptions)
        : this(options, hostApplicationLifetime, environment, hostOptions, NullLoggerFactory.Instance)
    {
    }

    public WpfApplicationLifetime(IOptions<WpfApplicationLifetimeOptions> options, IHostApplicationLifetime hostApplicationLifetime, IHostEnvironment environment, IOptions<HostOptions> hostOptions, ILoggerFactory loggerFactory)
    {
        Options = options.Value;
        HostOptions = hostOptions.Value;
        ApplicationLifetime = hostApplicationLifetime;
        Environment = environment;

        Logger = loggerFactory.CreateLogger("ZirconNet.WPF.Hosting.Lifetime");
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        if (!Options.SuppressStatusMessages)
        {
            _applicationStartedRegistration = ApplicationLifetime.ApplicationStarted.Register(state => ((WpfApplicationLifetime)state!).OnApplicationStarted(), this);
            _applicationStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state => ((WpfApplicationLifetime)state!).OnApplicationStopping(), this);
        }

        RegisterShutdownHandlers();

        return Task.CompletedTask;
    }

    private void OnApplicationStarted()
    {
        Logger.LogInformation("Application started.");
        Logger.LogInformation("Hosting environment: {envName}", Environment.EnvironmentName);
        Logger.LogInformation("Content root path: {contentRoot}", Environment.ContentRootPath);
    }

    private void OnApplicationStopping()
    {
        Logger.LogInformation("Application is shutting down...");
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        ApplicationLifetime.StopApplication();
        if (!_shutdownBlock.WaitOne(HostOptions.ShutdownTimeout))
        {
            Logger.LogInformation("Waiting for the host to be disposed. Ensure all 'IHost' instances are wrapped in 'using' blocks.");
        }

        _ = _shutdownBlock.WaitOne(HostOptions.ShutdownTimeout);

        System.Environment.ExitCode = 0;
    }

    private void UnregisterShutdownHandlers()
    {
        _ = _shutdownBlock.Set();

        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
    }

    private void RegisterShutdownHandlers()
    {
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        UnregisterShutdownHandlers();

        _applicationStartedRegistration?.Dispose();
        _applicationStoppingRegistration?.Dispose();
    }
}
