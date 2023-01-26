﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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