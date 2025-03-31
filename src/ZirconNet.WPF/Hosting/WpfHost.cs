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
    public static IHostBuilder CreateDefaultBuilder(string[] args)
    {
        var config = RegisterConfigurations(new ConfigurationBuilder()).Build();
        var builder = Host.CreateDefaultBuilder(args);
        builder.AddEnvironmentManager(args);

        builder.ConfigureAppConfiguration(builder => builder.AddConfiguration(config));

        return builder;
    }

    private static IConfigurationBuilder RegisterConfigurations(IConfigurationBuilder configuration)
    {
        var devStreamReader = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{AppDomain.CurrentDomain.FriendlyName}.appsettings.{Environment.GetEnvironmentVariable("DOTNET_")?.ToLower() ?? "production"}.json");

        configuration.AddJsonStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{AppDomain.CurrentDomain.FriendlyName}.appsettings.json")!);

        if (devStreamReader is not null)
        {
            configuration.AddJsonStream(devStreamReader);
        }

        configuration.AddEnvironmentVariables();

        return configuration;
    }
}
