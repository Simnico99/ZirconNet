// <copyright file="AddEnvironmentManagerExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ZirconNet.Core.Environments;

namespace ZirconNet.Microsoft.DependencyInjection.Hosting;

public static class AddEnvironmentManagerExtension
{
    public static IHostBuilder AddEnvironmentManager(this IHostBuilder hostBuilder, string[]? args = default)
    {
        EnvironmentManager.Current.SetEnvironmentFromStartupArgs(args);

        hostBuilder.ConfigureServices(services => services.TryAddSingleton<IEnvironmentManager, EnvironmentManagerDI>());
        return hostBuilder;
    }
}