// <copyright file="WpfHost.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ZirconNet.Microsoft.DependencyInjection.Hosting;

namespace ZirconNet.WPF.Hosting;

public sealed class WpfHost
{
#if NET6_0_OR_GREATER
    public static IHostApplicationBuilder CreateDefaultBuilder(string[] args)
    {
        var config = RegisterConfigurations(new ConfigurationBuilder()).Build();
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddEnvironmentManager(args);
        builder.Configuration.AddConfiguration(config);

        return builder;
    }
#else
    public static IHostBuilder CreateDefaultBuilder(string[] args)
    {
        var config = RegisterConfigurations(new ConfigurationBuilder()).Build();
        var builder = new HostBuilder().ConfigureDefaults(args);

        builder.AddEnvironmentManager(args);
        builder.ConfigureAppConfiguration(builder => builder.AddConfiguration(config));

        return builder;
    }
#endif

    private static IConfigurationBuilder RegisterConfigurations(IConfigurationBuilder configuration)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var devStreamReader = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.{Environment.GetEnvironmentVariable("DOTNET_")?.ToLower() ?? "production"}.json");
        var streamReader = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");

        if (streamReader is not null)
        {
            configuration.AddJsonStream(streamReader);
        }

        if (devStreamReader is not null)
        {
            configuration.AddJsonStream(devStreamReader);
        }

        return configuration;
    }
}
