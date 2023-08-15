// <copyright file="AddEnvironmentManagerExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZirconNet.Core.Environments;

namespace ZirconNet.Core.Hosting;

public static class AddEnvironmentManagerExtension
{
    public static IHostBuilder AddEnvironmentManager(this IHostBuilder hostBuilder, string[]? args = default)
    {
        EnvironmentManager.Current.SetEnvironmentFromStartupArgs(args);

        hostBuilder.ConfigureServices(services => services.AddSingleton<IEnvironmentManager, EnvironmentManagerDI>());

        hostBuilder.UseEnvironment(Environment.GetEnvironmentVariable("DOTNET_") ?? "Production");
        return hostBuilder;
    }
}
