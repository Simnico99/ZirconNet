// <copyright file="EnvironmentManagerDI.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Enums;
using ZirconNet.Core.Environments;

namespace ZirconNet.Microsoft.DependencyInjection.Hosting;

/// <inheritdoc cref="IEnvironmentManager"/>/>
internal sealed class EnvironmentManagerDI : IEnvironmentManager
{
    /// <inheritdoc/>
    public ApplicationEnvironment Environment => EnvironmentManager.Current.Environment;

    /// <inheritdoc/>
    public bool IsDebug => EnvironmentManager.Current.IsDebug;
}
